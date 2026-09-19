using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZNT.LevelEditor;

namespace ZNT.Evolution.Core;

public static class BaseComponentExtensions
{
    extension(BaseComponent component)
    {
        [UsedImplicitly]
        public void SetVisible(bool value)
        {
            typeof(Visibility).GetTypeInfo()
                .GetDeclaredField("visible")
                .SetValueDirect(__makeref(component.EditorVisibility), value);
        }

        [UsedImplicitly]
        public void SetDevOnly(bool value)
        {
            typeof(Visibility).GetTypeInfo()
                .GetDeclaredField("devOnly")
                .SetValueDirect(__makeref(component.EditorVisibility), value);
        }

        [UsedImplicitly]
        public void SetIgnoreSerialization(bool value)
        {
            typeof(Visibility).GetTypeInfo()
                .GetDeclaredField("ignoreSerialization")
                .SetValueDirect(__makeref(component.EditorVisibility), value);
        }

        [UsedImplicitly]
        public T Delegate<T>(string method) where T : System.Delegate
        {
            return System.Delegate.CreateDelegate(typeof(T), component, method) as T;
        }

        [UsedImplicitly]
        public T Event<T>(string name) where T : UnityEngine.Events.UnityEventBase
        {
            return Traverse.Create(component).Field("events").Field<T>(name).Value;
        }
    }

    extension(SerialIdentifier identifier)
    {
        [UsedImplicitly]
        public void SetSerialize(bool value)
        {
            Traverse.Create(identifier).Field<bool>("serialize").Value = value;
        }
    }

    extension(Trigger trigger)
    {
        [UsedImplicitly]
        public TriggerType Type
        {
            get => Traverse.Create(trigger).Field<TriggerType>("Type").Value;
            set => Traverse.Create(trigger).Field<TriggerType>("Type").Value = value;
        }

        [UsedImplicitly]
        public T GetEffect<T>() where T : TriggerEffect
        {
            var effect = trigger.gameObject.GetComponentSafe<T>();
            Traverse.Create(trigger).Field<TriggerEffect[]>("effects").Value = null;
            _ = trigger.Effects;
            return effect;
        }
    }

    extension(RayConeDetection vision)
    {
        [UsedImplicitly]
        public bool NeedUpdate => Traverse.Create(vision).Field<bool>("needUpdate").Value;

        [UsedImplicitly]
        public Vector3 PreviousForward => Traverse.Create(vision).Field<Vector3>("previousFoward").Value;

        [UsedImplicitly]
        public Vector2[] Rays => Traverse.Create(vision).Field<Vector2[]>("rays").Value;

        [UsedImplicitly]
        public int Inverted => Traverse.Create(vision).Field<int>("inverted").Value;
    }

    extension(LaserAttachment attachment)
    {
        [UsedImplicitly]
        public LayerMask ObstacleLayers
        {
            get => Traverse.Create(attachment).Field<LayerMask>("obstacleLayers").Value;
            set => Traverse.Create(attachment).Field<LayerMask>("obstacleLayers").Value = value;
        }
    }

    extension(SpawnPoint spawn)
    {
        [UsedImplicitly]
        public Parameters SendParams => Traverse.Create(spawn).Field<Parameters>("sendParams").Value;

        [UsedImplicitly]
        public int RandomSeed
        {
            get => Traverse.Create(spawn).Field<int>("randomSeed").Value;
            set => Traverse.Create(spawn).Field<int>("randomSeed").Value = value;
        }

        [UsedImplicitly]
        public string SpawnType => spawn switch
        {
            CharacterSpawnPoint => Traverse.Create(spawn).Field<System.Enum>("spawnType").Value.ToString(),
            { SpawnableObjects.Count: 0 } => null,
            not null when spawn.SpawnableObjects.All(asset => asset is HumanAsset) => "Human",
            not null when spawn.SpawnableObjects.All(asset => asset is ZombieAsset) => "Zombie",
            not null when spawn.SpawnableObjects.All(asset => asset is MovingObjectAsset) => "Movement",
            _ => null
        };

        [UsedImplicitly]
        public void ShowDirection(bool value)
        {
            Traverse.Create(spawn).Field("levelEditorOptions").Field<bool>("ShowDirection").Value = value;
        }

        [UsedImplicitly]
        public void ShowSpeed(bool value)
        {
            Traverse.Create(spawn).Field("levelEditorOptions").Field<bool>("ShowSpeed").Value = value;
        }

        [UsedImplicitly]
        public void ShowDuration(bool value)
        {
            Traverse.Create(spawn).Field("levelEditorOptions").Field<bool>("ShowDuration").Value = value;
        }

        [UsedImplicitly]
        public void ShowMoveOnStart(bool value)
        {
            Traverse.Create(spawn).Field("levelEditorOptions").Field<bool>("ShowMoveOnStart").Value = value;
        }

        [UsedImplicitly]
        public void ShowDamages(bool value)
        {
            Traverse.Create(spawn).Field("levelEditorOptions").Field<bool>("ShowDamages").Value = value;
        }
    }

    extension(Weapon weapon)
    {
        [UsedImplicitly]
        public void SetMagazine(int size)
        {
            weapon.DefaultMag = new Magazine(size);
            weapon.Initialize();
        }
    }

    extension(Stopper stopper)
    {
        [UsedImplicitly]
        public BoxDetection Detector => Traverse.Create(stopper).Field<BoxDetection>("detector").Value;

        [UsedImplicitly]
        public int MaxOpponents => Traverse.Create(stopper).Field<int>("MaxOpponents").Value;

        [UsedImplicitly]
        public bool BlockOpponents => Traverse.Create(stopper).Field<bool>("blockOpponents").Value;
    }

    extension(Rage rage)
    {
        [UsedImplicitly]
        public Timer Timer
        {
            get => Traverse.Create(rage).Field<Timer>("refillTimer").Value;
            set => Traverse.Create(rage).Field<Timer>("refillTimer").Value = value;
        }

        [UsedImplicitly]
        public int Hits
        {
            get => Traverse.Create(rage).Field<int>("currentHitCount").Value;
            set => Traverse.Create(rage).Field<int>("currentHitCount").Value = value;
        }

        [UsedImplicitly]
        public GameObject Repulse
        {
            get => Traverse.Create(rage).Field<GameObject>("repulse").Value;
            set => Traverse.Create(rage).Field<GameObject>("repulse").Value = value;
        }
    }

    extension(Moveable mover)
    {
        [UsedImplicitly]
        public float WalkSpeed
        {
            get => Traverse.Create(mover).Field<float>("walkSpeed").Value;
            set => Traverse.Create(mover).Field<float>("walkSpeed").Value = value;
        }

        [UsedImplicitly]
        public float RunSpeed
        {
            get => Traverse.Create(mover).Field<float>("runSpeed").Value;
            set => Traverse.Create(mover).Field<float>("runSpeed").Value = value;
        }

        [UsedImplicitly]
        public float SprintSpeed
        {
            get => Traverse.Create(mover).Field<float>("sprintSpeed").Value;
            set => Traverse.Create(mover).Field<float>("sprintSpeed").Value = value;
        }

        [UsedImplicitly]
        public void SetGroundLayers(LayerMask value)
        {
            Traverse.Create(mover).Field<LayerMask>("groundLayers").Value = value;
        }

        [UsedImplicitly]
        public void HitGround(float velocity)
        {
            Traverse.Create(mover).Method("HitGround", [typeof(float)], [velocity]).GetValue();
        }
    }

    extension(PropMoveable mover)
    {
        [UsedImplicitly]
        public void SetCurrentSpeed(float value)
        {
            Traverse.Create(mover).Field<float>("currentSpeed").Value = value;
        }
    }

    extension<T>(BehaviourAnimationController<T> controller) where T : BaseBehaviour
    {
        public T Behaviour => Traverse.Create(controller).Field<T>("Behaviour").Value;
    }

    extension(CharacterBehaviour behaviour)
    {
        [UsedImplicitly]
        public bool IsTalking()
        {
            return Traverse.Create(typeof(Dialogue))
                .Field<Dictionary<Transform, Dialogue>>("Talking").Value
                .ContainsKey(behaviour.transform);
        }

        [UsedImplicitly]
        public void Dialogue(LocalizableString text, float duration, Voice voice)
        {
            var dialogue = ComponentSingleton<GamePoolManager>.Instance
                .Spawn(nameof(Dialogue)).GetComponent<Dialogue>();
            var patroller = behaviour.Character.Components.Patroller;
            dialogue.SetText(text, duration);
            dialogue.Show(patroller, patroller.DialogueOffset, voice);
        }
    }

    extension(HumanBehaviour behaviour)
    {
        [UsedImplicitly]
        public void MoveToTarget(Transform target)
        {
            Traverse.Create(behaviour).Method("MoveToTarget", [typeof(Transform)], [target]).GetValue();
        }

        [UsedImplicitly]
        public void SetTarget(Transform target)
        {
            Traverse.Create(behaviour).Method("SetTarget", [typeof(Transform)], [target]).GetValue();
        }
    }

    extension(MovingObjectBehaviour behaviour)
    {
        [UsedImplicitly]
        public void ActivateColliders(bool active)
        {
            Traverse.Create(behaviour).Method("ActivateColliders", [typeof(bool)], [active]).GetValue();
        }
    }

    extension(PhysicObjectBehaviour behaviour)
    {
        [UsedImplicitly]
        public bool Exploded
        {
            get => Traverse.Create(behaviour).Field<bool>("exploded").Value;
            set => Traverse.Create(behaviour).Field<bool>("exploded").Value = value;
        }

        [UsedImplicitly]
        public void SendTargetDamage(GameObject target)
        {
            Traverse.Create(behaviour).Method("SendTargetDamage", [typeof(GameObject)], [target]).GetValue();
        }
    }

    extension(MineBehaviour behaviour)
    {
        [UsedImplicitly]
        public ExplosionAsset Explosion => Traverse.Create(behaviour).Field<ExplosionAsset>("explosion").Value;

        [UsedImplicitly]
        public Transform ExplosionPrefab
        {
            get => Traverse.Create(behaviour).Field<Transform>("explosionPrefab").Value;
            set => Traverse.Create(behaviour).Field<Transform>("explosionPrefab").Value = value;
        }

        [UsedImplicitly]
        public Trigger Trigger => Traverse.Create(behaviour).Field<Trigger>("trigger").Value;

        [UsedImplicitly]
        public MineAnimationController Animation =>
            Traverse.Create(behaviour).Field<MineAnimationController>("animation").Value;
    }

    extension(CorpseBehaviour behaviour)
    {
        [UsedImplicitly]
        public static Queue<CorpseBehaviour> AliveCorpses =>
            Traverse.Create<CorpseBehaviour>().Field<Queue<CorpseBehaviour>>("aliveCorpses").Value;

        [UsedImplicitly]
        public BoxCollider2D BoxCollider => Traverse.Create(behaviour).Field<BoxCollider2D>("boxCollider").Value;

        [UsedImplicitly]
        public CorpseParameter Parameters => Traverse.Create(behaviour).Field<CorpseParameter>("parameters").Value;
    }

    extension(SelectionMenu menu)
    {
        [UsedImplicitly]
        public EditorGameObject SerializeGameObject =>
            Traverse.Create(menu).Field<EditorGameObject>("serializeGameObject").Value;

        [UsedImplicitly]
        public SupportedTypePrefabs TypePrefabs =>
            Traverse.Create(menu).Field<SupportedTypePrefabs>("typePrefabs").Value;

        [UsedImplicitly]
        public SupportedTypePrefabs CustomDrawerPrefabs =>
            Traverse.Create(menu).Field<SupportedTypePrefabs>("customDrawerPrefabs").Value;


        [UsedImplicitly]
        public RectTransform MainContainer
        {
            get => Traverse.Create(menu).Field<RectTransform>("mainContainer").Value;
            set => Traverse.Create(menu).Field<RectTransform>("mainContainer").Value = value;
        }

        [UsedImplicitly]
        public ScrollRect ScrollRect =>
            Traverse.Create(menu).Field<ScrollRect>("scrollRect").Value;

        [UsedImplicitly]
        public InputField IdInputField =>
            Traverse.Create(menu).Field<InputField>("idInputField").Value;

        [UsedImplicitly]
        public Button ResetButton =>
            Traverse.Create(menu).Field<Button>("resetButton").Value;

        [UsedImplicitly]
        public Toggle MoveButton =>
            Traverse.Create(menu).Field<Toggle>("moveButton").Value;

        [UsedImplicitly]
        public Toggle CopyButton =>
            menu.MoveButton.transform.parent.Find("Copy Button")?.GetComponent<Toggle>();

        [UsedImplicitly]
        public Button DeleteButton =>
            Traverse.Create(menu).Field<Button>("deleteButton").Value;

        [UsedImplicitly]
        public Button DecorToBackButton =>
            Traverse.Create(menu).Field<Button>("decorToBackButton").Value;

        [UsedImplicitly]
        public InputField DecorIndexInputField =>
            Traverse.Create(menu).Field<InputField>("decorIndexInputField").Value;

        [UsedImplicitly]
        public Button DecorMinusButton =>
            Traverse.Create(menu).Field<Button>("decorMinusButton").Value;

        [UsedImplicitly]
        public Button DecorPlusButton =>
            Traverse.Create(menu).Field<Button>("decorPlusButton").Value;

        [UsedImplicitly]
        public Button DecorToFrontButton =>
            Traverse.Create(menu).Field<Button>("decorToFrontButton").Value;

        [UsedImplicitly]
        public SupportedTypeBinder TextBinder()
        {
            var prefab = menu.TypePrefabs[EditorComponent.SupportedType.String];
            return menu.InstantiateCustomBinder(prefab);
        }

        [UsedImplicitly]
        public SupportedTypeBinder ListBinder()
        {
            return menu.InstantiateCustomBinder(menu.CustomBinders.IntStringList);
        }

        [UsedImplicitly]
        public SupportedTypeBinder DirectionBinder()
        {
            var prefab = menu.CustomDrawerPrefabs[EditorComponent.SupportedType.Vector3];
            return menu.InstantiateCustomBinder(prefab);
        }
    }

    extension(SupportedTypeBinder binder)
    {
        [UsedImplicitly]
        public UIBehaviour[] UiComponents
        {
            get => Traverse.Create(binder).Field<UIBehaviour[]>("uiComponents").Value;
            set => Traverse.Create(binder).Field<UIBehaviour[]>("uiComponents").Value = value;
        }

        [UsedImplicitly]
        public Text Text => Traverse.Create(binder).Field<Text>("text").Value;
    }

    extension(LocalizableStringMenu localizable)
    {
        [UsedImplicitly]
        public bool UseStringCategory
        {
            get => Traverse.Create(localizable).Field<bool>("useStringCategory").Value;
            set => Traverse.Create(localizable).Field<bool>("useStringCategory").Value = value;
        }

        [UsedImplicitly]
        public string Category
        {
            get => Traverse.Create(localizable).Field<string>("category").Value;
            set => Traverse.Create(localizable).Field<string>("category").Value = value;
        }

        [UsedImplicitly]
        public CanvasGroup ToggleGroup => Traverse.Create(localizable).Field<CanvasGroup>("toggleGroup").Value;

        [UsedImplicitly]
        public Toggle LocalizeToggle => Traverse.Create(localizable).Field<Toggle>("localizeToggle").Value;

        [UsedImplicitly]
        public InputField ContentField => Traverse.Create(localizable).Field<InputField>("contentField").Value;

        [UsedImplicitly]
        public Text Placeholder => (Text)localizable.ContentField.placeholder;
    }

    extension(TutorialPageMenu page)
    {
        [UsedImplicitly]
        public LocalizableStringMenu TitleMenu =>
            Traverse.Create(page).Field<LocalizableStringMenu>("titleMenu").Value;

        [UsedImplicitly]
        public LocalizableStringMenu TextMenu =>
            Traverse.Create(page).Field<LocalizableStringMenu>("textMenu").Value;
    }

    extension(SignalReceiverLinker linker)
    {
        public void AddReceiver(ReceiverLink link)
        {
            Traverse.Create(linker).Method("AddReceiver", link.Component, link).GetValue();
        }
    }

    extension(SignalSenderLinker linker)
    {
        public void AddSender(SenderLink link)
        {
            Traverse.Create(linker).Method("AddSender", link.Component, link).GetValue();
        }
    }

    extension(WeatherRain weather)
    {
        [UsedImplicitly]
        public float Intensity
        {
            get => Traverse.Create(weather).Field<float>("intensity").Value;
            set => Traverse.Create(weather).Field<float>("intensity").Value = value;
        }

        [UsedImplicitly]
        public float Angle
        {
            get => Traverse.Create(weather).Field<float>("angle").Value;
            set => Traverse.Create(weather).Field<float>("angle").Value = value;
        }

        [UsedImplicitly]
        public Vector2 Speed
        {
            get => Traverse.Create(weather).Field<Vector2>("speed").Value;
            set => Traverse.Create(weather).Field<Vector2>("speed").Value = value;
        }

        [UsedImplicitly]
        public Vector4 Density
        {
            get => Traverse.Create(weather).Field<Vector4>("density").Value;
            set => Traverse.Create(weather).Field<Vector4>("density").Value = value;
        }

        [UsedImplicitly]
        public float Length
        {
            get => Traverse.Create(weather).Field<float>("length").Value;
            set => Traverse.Create(weather).Field<float>("length").Value = value;
        }

        [UsedImplicitly]
        public RainEffect RainEffect
        {
            get => Traverse.Create(weather).Field<RainEffect>("rainEffect").Value;
            set => Traverse.Create(weather).Field<RainEffect>("rainEffect").Value = value;
        }
    }

    extension(TutorialScreen screen)
    {
        [UsedImplicitly]
        public int NewsCount =>
            Traverse.Create(screen).Field<int>("newsCount").Value;

        [UsedImplicitly]
        public RectTransform NewsContainer =>
            Traverse.Create(screen).Field<RectTransform>("newsContainer").Value;

        [UsedImplicitly]
        public RectTransform NewsPrefab =>
            Traverse.Create(screen).Field<RectTransform>("newsPrefab").Value;

        [UsedImplicitly]
        public TutorialSettings TutorialSettings =>
            Traverse.Create(screen).Field<TutorialSettings>("tutorialSettings").Value;

        [UsedImplicitly]
        public Dictionary<string, TMPro.TextMeshProUGUI> CurrentNews =>
            Traverse.Create(screen).Field<Dictionary<string, TMPro.TextMeshProUGUI>>("currentNews").Value;

        [UsedImplicitly]
        public void AddScrollRecycler(RectTransform target)
        {
            Traverse.Create(screen).Method("AddScrollRecycler", [typeof(RectTransform)], [target]).GetValue();
        }
    }

    extension(ObjectSettings settings)
    {
        [UsedImplicitly]
        public LevelElement Element =>
            Traverse.Create(settings).Field<LevelElement>("element").Value;

        [UsedImplicitly]
        public LevelLoaderManager LevelManager =>
            Traverse.Create(settings).Field<LevelLoaderManager>("levelManager").Value;

        [UsedImplicitly]
        public Rotorz.Tile.TileSystem TileSystem =>
            Traverse.Create(settings).Field<Rotorz.Tile.TileSystem>("tileSystem").Value;
    }
}