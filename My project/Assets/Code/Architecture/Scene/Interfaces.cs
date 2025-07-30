
//Service
public interface IInitializable { void OnInitialize(); }
public interface IUpdateble { public void OnUpdate(); }
public interface ILogicUpdateble { public void OnLogicUpdate(float dt); }
public interface ICleanup { void OnCleanup(); }



public interface IAwakeObjectModule { void OnAwakeObject(ObjectState state); }
public interface IStartObjectModule { void OnStartObject(ObjectState state); }
public interface ILogicUpdateObjectModule { public void OnLogicUpdateObject(ObjectState state, float dt); }
public interface IDestroyObjectModule { void OnDestroyObject(ObjectState state); }


