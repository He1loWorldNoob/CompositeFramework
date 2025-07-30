
using UnityEngine;

public class TestUnitEntity : CmsEntity
{
    public TestUnitEntity()
    {
        Define<MoveSpeedTag>().speed = 10;
        var prefab = Resources.Load<GameObject>("Prefabs/TestUnit");
        Define<TagPrefab>().prefab = prefab.GetComponent<ObjectView>();
    }
}

public class InitializeUnitSystem : BaseInteraction, IInitializable
{
    private ObjectContainer _objectContainer;

    public InitializeUnitSystem(ObjectContainer objectContainer)
    {
        _objectContainer = objectContainer;
    }

    public void OnInitialize()
    {
        var unit = _objectContainer.Create(CMS.Get<TestUnitEntity>());
        unit.Define<TransformComponent>().position = new Vector3(0, 0, 0);
    }
}