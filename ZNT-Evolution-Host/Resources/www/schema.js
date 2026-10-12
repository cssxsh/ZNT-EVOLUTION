/* =========================================================
 *  类型定义（Schema）
 *  ---------------------------------------------------------
 *  keys 中每个键可定义为：
 *    { type: 'text',    desc, placeholder }
 *    { type: 'pattern', desc, pattern, message, placeholder }
 *    { type: 'enum',    desc, options: [...] }
 *    { type: 'number',  desc, numberKind: 'int'|'float', min, max, step, placeholder }
 *  键值必须是上面的对象写法
 * ======================================================= */

const SCHEMA = {
    HumanAsset: {
        desc: "人类 / 角色类实体资源",
        keys: {
            RiseAsset: {
                desc: "要替换的资源",
                type: "pattern",
                pattern: "^\\s*[A-Za-z_][A-Za-z0-9_]*\\s*:\\s*[A-Za-z_][A-Za-z0-9_]*\\s*$",
                placeholder: "SwordWomen : HumanAsset",
                message: '格式应为 "资源名 : 类型名"，例如 SwordWomen : HumanAsset'
            },
            Faction: { desc: "阵营归属", type: "enum", options: ["Neutral", "Friendly", "Hostile"] },
            MoveSpeed: { desc: "移动速度倍率", type: "number", numberKind: "float", min: 0, max: 5, step: 0.05 },
            AmmoCount: { desc: "弹药数量", type: "number", numberKind: "int", min: 0, max: 9999, step: 1 }
        }
    },
    VehicleAsset: {
        desc: "载具类实体资源",
        keys: {
            RiseAsset: {
                desc: "要替换的资源",
                type: "pattern",
                pattern: "^\\s*[A-Za-z_][A-Za-z0-9_]*\\s*:\\s*[A-Za-z_][A-Za-z0-9_]*\\s*$",
                placeholder: "Tank : VehicleAsset",
                message: '格式应为 "资源名 : 类型名"'
            },
            Durability: { desc: "耐久系数", type: "number", numberKind: "float", min: 0, max: 10, step: 0.1 }
        }
    }
};
