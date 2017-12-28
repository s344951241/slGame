using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityMgr :Singleton<EntityMgr> {

    private Dictionary<Transform, EntityBase> _dicEntityTran;
    private Dictionary<int, EntityBase> _dicEntityId;

    public IEnumerable<EntityBase> EntityList {
        get {
            return _dicEntityTran.Values;
        }
    }

    public EntityMgr()
    {
        _dicEntityTran = new Dictionary<Transform, EntityBase>();
        _dicEntityId = new Dictionary<int, EntityBase>();
        Creator();
    }
    public void AddTransformDic(EntityBase kEnt)
    {
        _dicEntityTran.Add(kEnt.transform, kEnt);
        _dicEntityId.Add(kEnt.roleKey, kEnt);
    }
    public EntityBase CreateEntity(int key, string roleId)
    {
        EntityBase entitybase = EntityBase.Creator();
        entitybase.roleKey = key;
        //entitybase.PrefabId = roleId;
        entitybase.Reset();
        return entitybase;
    }
    public void Creator()
    {
        GlobalTimer.GetInstance().update = delegate(float dt)
        {
            onUpdate(dt);
        };
    }
    public void onUpdate(float dt)
    {
        foreach (var item in _dicEntityTran)
        {
            item.Value.OnUpdate(dt);
        }
    }

    public EntityBase GetEntity(Transform tran)
    { 
        EntityBase result;
        _dicEntityTran.TryGetValue(tran,out result);
        return result;
    }

    public EntityBase GetEntity(GameObject obj)
    {
        return GetEntity(obj.transform);
    }
    public EntityBase GetEntity(int roleKey)
    {
        return _dicEntityId[roleKey];
    }
    public void Reclaim(EntityBase kEnt)
    {
        if (_dicEntityTran.ContainsKey(kEnt.transform))
        {
            _dicEntityTran.Remove(kEnt.transform);
        }
        if (_dicEntityId.ContainsKey(kEnt.roleKey))
        {
            _dicEntityId.Remove(kEnt.roleKey);
        }
        kEnt.Reset();
    }

    public void RecaimAll()
    {
        foreach (var item in _dicEntityTran)
        {
            Reclaim(item.Value);
        }
    }
    public void RemoveDic(EntityBase kEnt)
    {
        if (_dicEntityTran.ContainsKey(kEnt.transform))
            _dicEntityTran.Remove(kEnt.transform);
        if (_dicEntityId.ContainsKey(kEnt.roleKey))
            _dicEntityId.Remove(kEnt.roleKey);
    }
}
