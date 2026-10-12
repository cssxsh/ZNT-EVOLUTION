/* =========================================================
 *  AssetOverride 编辑器
 *  ---------------------------------------------------------
 *  类型定义（Schema）见同目录 schema.js
 * ======================================================= */

const schema = normalizeSchema(SCHEMA);
let overrides = [];
let uidSeq = 0;

const listEl   = document.getElementById("list");
const jsonEl   = document.getElementById("jsonOut");
const statsEl  = document.getElementById("stats");
const editorEl = document.getElementById("editor");
const modalEl  = document.getElementById("modal");

/* =========================================================
 *  工具函数
 * ======================================================= */

const newUid = () => "u" + (++uidSeq);

function el(tag, cls, text) {
    const n = document.createElement(tag);
    if (cls) n.className = cls;
    if (text != null) n.textContent = text;
    return n;
}

function iconBtn(text, title, onClick, extraClass = "") {
    const b = el("button", "icon-btn " + extraClass, text);
    b.type = "button";
    if (title) b.title = title;
    if (onClick) b.addEventListener("click", onClick);
    return b;
}

/* =========================================================
 *  Schema 规范化与查询
 * ======================================================= */

function normalizeSchema(src) {
    const out = {};
    if (!src || typeof src !== "object" || Array.isArray(src)) return out;

    Object.keys(src).forEach(typeName => {
        const def = src[typeName];
        if (!def || typeof def !== "object") return;

        const entry = { desc: def.desc || "", keys: {} };
        const rawKeys = def.keys && typeof def.keys === "object" ? def.keys : {};

        Object.keys(rawKeys).forEach(keyName => {
            const v = rawKeys[keyName];
            if (!v || typeof v !== "object") return;

            const kd = Object.assign({}, v);
            kd.type = kd.type || "text";
            entry.keys[keyName] = kd;
        });

        out[typeName] = entry;
    });

    return out;
}

function getKeyDef(type, key) {
    const s = schema[type];
    if (!s || !s.keys) return null;
    const k = (key || "").trim();
    if (!k) return null;
    return s.keys[k] || null;
}

function typeDesc(type) {
    const s = schema[type];
    return s && s.desc ? s.desc : "";
}

/* =========================================================
 *  数值步进工具
 * ======================================================= */

function getStepValue(kd) {
    if (kd && kd.step != null && isFinite(Number(kd.step)) && Number(kd.step) > 0) {
        return Number(kd.step);
    }
    return (kd && kd.numberKind === "int") ? 1 : 0.1;
}

function decimalsOf(n) {
    const s = String(n);
    if (/e/i.test(s)) return 6;
    const i = s.indexOf(".");
    return i < 0 ? 0 : Math.min(s.length - i - 1, 10);
}

function roundTo(v, d) {
    return Number(v.toFixed(Math.min(d, 10)));
}

function parseNumeric(raw) {
    const s = String(raw == null ? "" : raw).trim();
    if (s === "") return null;
    const n = Number(s);
    return Number.isFinite(n) ? n : null;
}

/**
 * 给按钮绑定「单击 + 长按连发」。
 * - 指针按下立即触发一次
 * - 按住 420ms 后，每 55ms 触发一次
 * - 松开 / 取消时停止
 * - 键盘（Enter / Space）触发的 click（detail === 0）也可用
 */
function bindStepButton(btn, action) {
    let delayT = null;
    let repeatT = null;

    const stop = () => {
        if (delayT !== null) { clearTimeout(delayT); delayT = null; }
        if (repeatT !== null) { clearInterval(repeatT); repeatT = null; }
        document.removeEventListener("pointerup", stop);
        document.removeEventListener("pointercancel", stop);
    };

    btn.addEventListener("pointerdown", (e) => {
        if (btn.disabled) return;
        if (e.button != null && e.button !== 0) return;  // 仅左键 / 触摸
        action();

        document.addEventListener("pointerup", stop);
        document.addEventListener("pointercancel", stop);

        delayT = setTimeout(() => {
            delayT = null;
            repeatT = setInterval(() => {
                if (btn.disabled) { stop(); return; }
                action();
            }, 55);
        }, 420);
    });

    // 键盘可访问性：键盘触发的 click 其 detail 为 0
    btn.addEventListener("click", (e) => {
        if (e.detail === 0 && !btn.disabled) action();
    });
}

/* =========================================================
 *  值校验
 * ======================================================= */

function validateValue(kd, raw, vtype) {
    const val = raw == null ? "" : String(raw);

    if (vtype === "pattern") {
        if (val === "") return { ok: false, msg: "不能为空" };
        if (kd.pattern) {
            let re;
            try { re = new RegExp(kd.pattern); }
            catch (e) { return { ok: true }; }
            if (!re.test(val)) {
                return { ok: false, msg: kd.message || "格式不符合要求" };
            }
        }
        return { ok: true };
    }

    if (vtype === "enum") {
        const opts = (kd.options || []).map(String);
        if (!opts.length) return { ok: true };
        if (!opts.includes(val)) {
            return { ok: false, msg: "需为：" + opts.join(" / ") };
        }
        return { ok: true };
    }

    if (vtype === "number") {
        if (val.trim() === "") return { ok: false, msg: "不能为空" };
        const n = Number(val);
        if (!Number.isFinite(n)) return { ok: false, msg: "不是有效数字" };
        if (kd.numberKind === "int" && !Number.isInteger(n)) {
            return { ok: false, msg: "必须是整数" };
        }
        if (kd.min != null && n < Number(kd.min)) {
            return { ok: false, msg: "不能小于 " + kd.min };
        }
        if (kd.max != null && n > Number(kd.max)) {
            return { ok: false, msg: "不能大于 " + kd.max };
        }
        return { ok: true };
    }

    return { ok: true };
}

function effectiveType(kd) {
    return (kd && kd.type) ? kd.type : "text";
}

/* =========================================================
 *  数据 <-> JSON
 * ======================================================= */

function newOverride(Type = "", Pattern = "", handles = []) {
    return {
        uid: newUid(),
        Type: Type,
        Pattern: Pattern,
        handles: handles.map(h => ({ uid: newUid(), key: h.key || "", value: h.value == null ? "" : String(h.value) }))
    };
}

function normalizeHandles(h) {
    if (!h || typeof h !== "object" || Array.isArray(h)) return [];
    return Object.entries(h).map(([key, value]) => ({
        key,
        value: typeof value === "string" ? value : JSON.stringify(value)
    }));
}

function fromJSON(arr) {
    return arr.map(o => newOverride(
        o.Type ?? o.type ?? "",
        o.Pattern ?? o.pattern ?? "",
        normalizeHandles(o.Handles ?? o.handles)
    ));
}

function convertValueForJSON(type, key, raw) {
    const kd = getKeyDef(type, key);
    if (kd && kd.type === "number") {
        const n = parseNumeric(raw);
        if (n !== null) return n;
    }
    return raw;
}

function toJSON() {
    return overrides.map(o => {
        const Handles = {};
        o.handles.forEach(h => {
            const k = (h.key || "").trim();
            if (!k) return;
            Handles[k] = convertValueForJSON(o.Type, k, h.value);
        });
        return { Type: o.Type, Pattern: o.Pattern, Handles };
    });
}

/* =========================================================
 *  预览刷新
 * ======================================================= */

function refreshPreview() {
    updateJSON();
    updateStats();
}

/* =========================================================
 *  键名建议
 * ======================================================= */

function knownKeysFor(type) {
    const set = new Set();
    const s = schema[type];
    if (s && s.keys) Object.keys(s.keys).forEach(k => set.add(k));
    overrides.forEach(o => {
        if (o.Type === type) o.handles.forEach(h => { if (h.key) set.add(h.key); });
    });
    return [...set];
}

function refreshTypeOptions() {
    const dl = document.getElementById("typeOptions");
    dl.innerHTML = "";
    const types = new Set(Object.keys(schema));
    overrides.forEach(o => { if (o.Type) types.add(o.Type); });
    [...types].sort().forEach(t => {
        const opt = document.createElement("option");
        opt.value = t;
        dl.appendChild(opt);
    });
}

function fillKeyDatalist(dl, type) {
    if (!dl) return;
    dl.innerHTML = "";
    knownKeysFor(type).forEach(k => {
        const opt = document.createElement("option");
        opt.value = k;
        dl.appendChild(opt);
    });
}

/* =========================================================
 *  渲染入口
 * ======================================================= */

function render() {
    const scrollTop = editorEl.scrollTop;
    listEl.innerHTML = "";

    if (!overrides.length) {
        const empty = el("div", "empty");
        empty.appendChild(el("div", "big", "◇"));
        empty.appendChild(el("p", null, "还没有任何覆盖项"));
        const btn = el("button", "btn primary", "载入示例数据");
        btn.addEventListener("click", loadSample);
        empty.appendChild(btn);
        listEl.appendChild(empty);
    } else {
        overrides.forEach((ov, i) => listEl.appendChild(buildCard(ov, i)));
    }

    refreshTypeOptions();
    updateJSON();
    updateStats();
    editorEl.scrollTop = scrollTop;
}

/* =========================================================
 *  构建值控件
 * ======================================================= */

function buildValueControl(ov, h, kd, vtype) {
    const cell = el("div", "value-cell");
    const errEl = el("div", "value-error");

    let control = null;   // 用于 classList 操作的元素
    let node = null;      // 真正插入 cell 的元素

    const setError = (msg) => {
        if (msg) {
            errEl.textContent = msg;
            if (control) control.classList.add("invalid");
        } else {
            errEl.textContent = "";
            if (control) control.classList.remove("invalid");
        }
        updateStats();
    };

    const validateNow = () => {
        if (vtype === "text") { setError(""); return; }
        const r = validateValue(kd, h.value, vtype);
        setError(r.ok ? "" : r.msg);
    };

    /* ---------------- 枚举 ---------------- */
    if (vtype === "enum") {
        const sel = el("select", "input small mono");
        const opts = (kd.options || []).map(String);

        const ph = document.createElement("option");
        ph.value = "";
        ph.textContent = "请选择…";
        sel.appendChild(ph);

        opts.forEach(v => {
            const o = document.createElement("option");
            o.value = v;
            o.textContent = v;
            sel.appendChild(o);
        });

        const cur = h.value;
        if (cur !== "" && !opts.includes(cur)) {
            const o = document.createElement("option");
            o.value = cur;
            o.textContent = cur + "（非标准值）";
            sel.appendChild(o);
        }

        sel.value = cur;
        sel.title = kd.desc || "";
        sel.addEventListener("change", () => {
            h.value = sel.value;
            validateNow();
            refreshPreview();
        });

        control = sel;
        node = sel;
    }

    /* ---------------- 数值（带进 / 退步按钮） ---------------- */
    else if (vtype === "number") {
        const step = getStepValue(kd);
        const dec  = decimalsOf(step);

        const wrap = el("div", "number-row");

        const minusBtn = iconBtn("−", "减 " + step + "（按住可连续）", null, "step-btn");
        const plusBtn  = iconBtn("+", "加 " + step + "（按住可连续）", null, "step-btn");

        const input = el("input", "input small mono");
        input.type = "number";
        input.step = String(step);
        if (kd.min != null) input.min = kd.min;
        if (kd.max != null) input.max = kd.max;
        input.value = h.value;

        const lo = kd.min != null ? kd.min : "-∞";
        const hi = kd.max != null ? kd.max : "∞";
        const range = (kd.min != null || kd.max != null) ? `${lo} ~ ${hi}` : "";
        input.placeholder = kd.placeholder || range ||
            (kd.numberKind === "int" ? "整数" : "数值");
        input.title = (kd.desc ? kd.desc + "  " : "") +
            (range ? `[${range}] 步进 ${step}` : `步进 ${step}`);

        /* 边界时禁用按钮 */
        const updateButtons = () => {
            const cur = parseNumeric(input.value);
            if (cur === null) {
                minusBtn.disabled = false;
                plusBtn.disabled = false;
                return;
            }
            minusBtn.disabled = kd.min != null && cur <= Number(kd.min);
            plusBtn.disabled  = kd.max != null && cur >= Number(kd.max);
        };

        /* 步进 */
        const doStep = (dir) => {
            const cur = parseNumeric(input.value);
            let base;
            if (cur !== null) base = cur;
            else if (kd.min != null) base = Number(kd.min);
            else base = 0;

            let next = roundTo(base + dir * step, dec);
            if (kd.numberKind === "int") next = Math.round(next);
            if (kd.min != null && next < Number(kd.min)) next = Number(kd.min);
            if (kd.max != null && next > Number(kd.max)) next = Number(kd.max);

            const str = String(next);
            if (str === input.value) { updateButtons(); return; }

            input.value = str;
            h.value = str;
            validateNow();
            refreshPreview();
            updateButtons();
        };

        bindStepButton(minusBtn, () => doStep(-1));
        bindStepButton(plusBtn,  () => doStep(1));

        input.addEventListener("input", () => {
            h.value = input.value;
            validateNow();
            updateButtons();
            refreshPreview();
        });

        wrap.append(minusBtn, input, plusBtn);

        control = input;
        node = wrap;

        // 初始化按钮状态
        setTimeout(updateButtons, 0);
        updateButtons();
    }

    /* ---------------- 正则受限文本 ---------------- */
    else if (vtype === "pattern") {
        const input = el("input", "input small mono");
        input.type = "text";
        input.value = h.value;
        input.placeholder = kd.placeholder || "格式受限";
        input.title = kd.desc || "";
        input.addEventListener("input", () => {
            h.value = input.value;
            validateNow();
            refreshPreview();
        });

        control = input;
        node = input;
    }

    /* ---------------- 自由文本 ---------------- */
    else {
        const input = el("input", "input small mono");
        input.type = "text";
        input.value = h.value;
        input.placeholder = kd && kd.placeholder ? kd.placeholder : "值";
        input.title = kd && kd.desc ? kd.desc : "";
        input.addEventListener("input", () => {
            h.value = input.value;
            refreshPreview();
        });

        control = input;
        node = input;
    }

    cell.appendChild(node);
    cell.appendChild(errEl);
    validateNow();
    return cell;
}

/* =========================================================
 *  构建 Handle 行
 * ======================================================= */

function buildHandleRow(ov, h, dlId, onStructureChange) {
    const row = el("div", "handle-row");

    const keyInput = el("input", "input small mono");
    keyInput.type = "text";
    keyInput.value = h.key;
    keyInput.placeholder = "键名";
    if (dlId) keyInput.setAttribute("list", dlId);

    const valueCell = el("div", "value-cell");

    const delBtn = iconBtn("✕", "删除该键值对", () => {
        ov.handles = ov.handles.filter(x => x !== h);
        onStructureChange();
    }, "danger");

    function renderValue() {
        const kd = getKeyDef(ov.Type, h.key);
        const vtype = effectiveType(kd);
        valueCell.innerHTML = "";
        valueCell.appendChild(buildValueControl(ov, h, kd, vtype));
    }

    keyInput.addEventListener("input", () => {
        h.key = keyInput.value;
        renderValue();
        refreshPreview();
    });

    row._renderValue = renderValue;
    row.append(keyInput, valueCell, delBtn);
    renderValue();
    return row;
}

/* =========================================================
 *  构建卡片
 * ======================================================= */

function buildCard(ov, index) {
    const card = el("div", "card");
    card.dataset.uid = ov.uid;

    const dl = el("datalist");
    dl.id = "keys-" + ov.uid;
    card.appendChild(dl);

    /* ---------- 头部一行 ---------- */
    const head = el("div", "card-head");
    head.appendChild(el("span", "badge", String(index + 1)));

    const typeField = el("div", "field");
    typeField.appendChild(el("span", "lbl", "Type"));
    const typeInput = el("input", "input");
    typeInput.type = "text";
    typeInput.value = ov.Type;
    typeInput.placeholder = "例如 HumanAsset";
    typeInput.setAttribute("list", "typeOptions");
    typeField.appendChild(typeInput);
    head.appendChild(typeField);

    const patField = el("div", "field wide");
    patField.appendChild(el("span", "lbl", "Pattern"));
    const patInput = el("input", "input mono");
    patInput.type = "text";
    patInput.value = ov.Pattern;
    patInput.placeholder = "例如 CivilianFemale.*";
    patField.appendChild(patInput);
    head.appendChild(patField);

    const ops = el("div", "card-ops");
    ops.append(
        iconBtn("↑", "上移", () => moveCard(ov.uid, -1)),
        iconBtn("↓", "下移", () => moveCard(ov.uid, 1)),
        iconBtn("⧉", "复制", () => duplicateCard(ov)),
        iconBtn("✕", "删除", () => {
            overrides = overrides.filter(o => o.uid !== ov.uid);
            render();
        }, "danger")
    );
    head.appendChild(ops);
    card.appendChild(head);

    /* ---------- 正则状态行 ---------- */
    const statusRow = el("div", "pattern-status");
    const statusEl = el("span", "status");
    const testWrap = el("label", "test-wrap");
    testWrap.appendChild(el("span", "lbl-inline", "测试"));
    const testInput = el("input", "input small mono");
    testInput.type = "text";
    testInput.placeholder = "输入一段文本，测试能否匹配";
    testWrap.appendChild(testInput);
    const matchEl = el("span", "match-info");
    statusRow.append(statusEl, testWrap, matchEl);
    card.appendChild(statusRow);

    /* ---------- Handles ---------- */
    const handlesSec = el("div", "handles");

    const hHead = el("div", "handles-head");
    hHead.appendChild(el("span", "section-title", "Handles"));
    const hintEl = el("span", "hint");
    hHead.appendChild(hintEl);
    const addBtn = el("button", "btn tiny", "＋ 添加键值对");
    addBtn.type = "button";
    addBtn.addEventListener("click", () => {
        ov.handles.push({ uid: newUid(), key: "", value: "" });
        render();
    });
    hHead.appendChild(addBtn);
    handlesSec.appendChild(hHead);

    const rows = [];

    if (!ov.handles.length) {
        handlesSec.appendChild(el("div", "handles-empty", "暂无键值对，点击「添加键值对」开始"));
    }

    ov.handles.forEach(h => {
        const r = buildHandleRow(ov, h, dl.id, () => render());
        rows.push(r);
        handlesSec.appendChild(r);
    });

    card.appendChild(handlesSec);

    /* ---------- 行为绑定 ---------- */

    typeInput.addEventListener("input", () => {
        ov.Type = typeInput.value;
        fillKeyDatalist(dl, ov.Type);
        hintEl.textContent = typeDesc(ov.Type);
        rows.forEach(r => r._renderValue && r._renderValue());
        refreshPreview();
    });

    typeInput.addEventListener("change", () => {
        refreshTypeOptions();
    });

    patInput.addEventListener("input", () => {
        ov.Pattern = patInput.value;
        updatePatternStatus();
        refreshPreview();
    });

    testInput.addEventListener("input", updatePatternStatus);

    function updatePatternStatus() {
        const p = ov.Pattern;
        let err = "";

        if (!p.trim()) {
            err = "正则表达式为空";
        } else {
            try { new RegExp(p); } catch (e) { err = e.message; }
        }

        if (err) {
            statusEl.className = "status err";
            statusEl.textContent = "✕ " + err;
            patInput.classList.add("invalid");
            testInput.disabled = true;
            matchEl.textContent = "";
            return;
        }

        statusEl.className = "status ok";
        statusEl.textContent = "✓ 正则有效";
        patInput.classList.remove("invalid");
        testInput.disabled = false;

        const t = testInput.value;
        if (!t) { matchEl.textContent = ""; return; }

        try {
            const m = t.match(new RegExp(ov.Pattern, "g"));
            if (m && m.length) {
                const shown = m.slice(0, 3).map(x => JSON.stringify(x));
                matchEl.textContent = `匹配 ${m.length} 处：${shown.join("、")}${m.length > 3 ? " …" : ""}`;
            } else {
                matchEl.textContent = "无匹配";
            }
        } catch (e) {
            matchEl.textContent = "";
        }
    }

    hintEl.textContent = typeDesc(ov.Type);
    fillKeyDatalist(dl, ov.Type);
    updatePatternStatus();

    return card;
}

/* =========================================================
 *  卡片操作
 * ======================================================= */

function moveCard(uid, dir) {
    const i = overrides.findIndex(o => o.uid === uid);
    const j = i + dir;
    if (i < 0 || j < 0 || j >= overrides.length) return;
    [overrides[i], overrides[j]] = [overrides[j], overrides[i]];
    render();
}

function duplicateCard(ov) {
    const idx = overrides.findIndex(o => o.uid === ov.uid);
    const copy = newOverride(ov.Type, ov.Pattern, ov.handles.map(h => ({ key: h.key, value: h.value })));
    overrides.splice(idx + 1, 0, copy);
    render();
}

function loadSample() {
    overrides = [
        newOverride("HumanAsset", "CivilianFemale.*", [
            { key: "RiseAsset",  value: "SwordWomen : HumanAsset" },
            { key: "Faction",    value: "Friendly" },
            { key: "MoveSpeed",  value: "1.2" },
            { key: "AmmoCount",  value: "30" }
        ])
    ];
    render();
}

/* =========================================================
 *  JSON 预览
 * ======================================================= */

function highlightJSON(json) {
    return json
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(
            /("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g,
            (m) => {
                let cls = "j-num";
                if (/^"/.test(m)) {
                    cls = /:$/.test(m) ? "j-key" : "j-str";
                } else if (/true|false/.test(m)) {
                    cls = "j-bool";
                } else if (/null/.test(m)) {
                    cls = "j-null";
                }
                return `<span class="${cls}">${m}</span>`;
            }
        );
}

function updateJSON() {
    jsonEl.innerHTML = highlightJSON(JSON.stringify(toJSON(), null, 2));
}

function updateStats() {
    let issues = 0;

    overrides.forEach(o => {
        if (!o.Type.trim()) issues++;
        if (!o.Pattern.trim()) issues++;
        else { try { new RegExp(o.Pattern); } catch (e) { issues++; } }

        o.handles.forEach(h => {
            const k = (h.key || "").trim();
            if (!k) return;
            const kd = getKeyDef(o.Type, k);
            const vtype = effectiveType(kd);
            if (vtype === "text") return;
            const r = validateValue(kd, h.value, vtype);
            if (!r.ok) issues++;
        });
    });

    if (!overrides.length) {
        statsEl.textContent = "空";
        statsEl.className = "stats";
    } else if (issues) {
        statsEl.textContent = `${overrides.length} 项 · ${issues} 个问题`;
        statsEl.className = "stats warn";
    } else {
        statsEl.textContent = `${overrides.length} 项 · 全部有效`;
        statsEl.className = "stats ok";
    }
}

/* =========================================================
 *  模态框
 * ======================================================= */

function showModal(title, bodyEl, footButtons) {
    document.getElementById("modalTitle").textContent = title;
    const body = document.getElementById("modalBody");
    const foot = document.getElementById("modalFoot");
    body.innerHTML = "";
    foot.innerHTML = "";
    body.appendChild(bodyEl);
    footButtons.forEach(b => foot.appendChild(b));
    modalEl.classList.remove("hidden");
}

function hideModal() {
    modalEl.classList.add("hidden");
}

document.getElementById("modalClose").addEventListener("click", hideModal);
modalEl.addEventListener("click", e => { if (e.target === modalEl) hideModal(); });
document.addEventListener("keydown", e => {
    if (e.key === "Escape" && !modalEl.classList.contains("hidden")) hideModal();
});

/* ---------- 导入 ---------- */

function openImport() {
    const body = el("div", "stack");
    body.appendChild(el("p", "muted", "粘贴 AssetOverride 的 JSON 数组，或从文件载入。"));

    const ta = el("textarea", "input mono modal-textarea");
    ta.placeholder =
        '[\n  {\n    "Type": "HumanAsset",\n' +
        '    "Pattern": "CivilianFemale.*",\n' +
        '    "Handles": { "RiseAsset": "SwordWomen : HumanAsset" }\n' +
        '  }\n]';
    body.appendChild(ta);

    const errEl = el("p", "error-msg");
    body.appendChild(errEl);

    const fileLabel = el("label", "btn small file-btn", "选择文件…");
    const fileInput = document.createElement("input");
    fileInput.type = "file";
    fileInput.accept = ".json,application/json";
    fileInput.addEventListener("change", () => {
        const f = fileInput.files[0];
        if (!f) return;
        const reader = new FileReader();
        reader.onload = () => { ta.value = String(reader.result); errEl.textContent = ""; };
        reader.readAsText(f);
    });
    fileLabel.appendChild(fileInput);

    const row = el("div", "row");
    row.appendChild(fileLabel);
    body.appendChild(row);

    const cancelBtn = el("button", "btn", "取消");
    cancelBtn.addEventListener("click", hideModal);

    const okBtn = el("button", "btn primary", "导入");
    okBtn.addEventListener("click", () => {
        const raw = ta.value.trim();
        if (!raw) { errEl.textContent = "内容为空"; return; }

        let parsed;
        try { parsed = JSON.parse(raw); }
        catch (e) { errEl.textContent = "JSON 解析失败：" + e.message; return; }

        const arr = Array.isArray(parsed) ? parsed
            : (parsed && Array.isArray(parsed.Overrides) ? parsed.Overrides : null);
        if (!arr) { errEl.textContent = "顶层必须是一个数组"; return; }

        overrides = fromJSON(arr);
        hideModal();
        render();
    });

    showModal("导入 AssetOverride", body, [cancelBtn, okBtn]);
}

/* =========================================================
 *  顶部按钮
 * ======================================================= */

document.getElementById("btnAdd").addEventListener("click", () => {
    overrides.push(newOverride("", "", []));
    render();

    const cards = listEl.querySelectorAll(".card");
    const last = cards[cards.length - 1];
    if (last) {
        last.scrollIntoView({ behavior: "smooth", block: "nearest" });
        const input = last.querySelector(".field .input");
        if (input) input.focus();
    }
});

document.getElementById("btnImport").addEventListener("click", openImport);

document.getElementById("btnExport").addEventListener("click", () => {
    const blob = new Blob([JSON.stringify(toJSON(), null, 2)], { type: "application/json" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = "AssetOverrides.json";
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
});

document.getElementById("btnCopy").addEventListener("click", async (e) => {
    const btn = e.currentTarget;
    const text = JSON.stringify(toJSON(), null, 2);
    try {
        await navigator.clipboard.writeText(text);
    } catch (err) {
        const ta = document.createElement("textarea");
        ta.value = text;
        ta.style.position = "fixed";
        ta.style.opacity = "0";
        document.body.appendChild(ta);
        ta.select();
        document.execCommand("copy");
        document.body.removeChild(ta);
    }
    const old = btn.textContent;
    btn.textContent = "已复制 ✓";
    setTimeout(() => { btn.textContent = old; }, 1200);
});

document.getElementById("btnClear").addEventListener("click", () => {
    if (!overrides.length) return;
    if (!confirm("确定清空所有覆盖项吗？此操作不可撤销。")) return;
    overrides = [];
    render();
});

/* =========================================================
 *  启动
 * ======================================================= */

render();
