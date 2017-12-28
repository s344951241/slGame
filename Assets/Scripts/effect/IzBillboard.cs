using Engine;
using System;
using UnityEngine;

public class IzBillboard : MonoBehaviour
{
    //
    // Fields
    //
    public IzBillboard.BILLBOARD_TYPE _BillboardType = IzBillboard.BILLBOARD_TYPE.PERPENDICUAL_VIEW_Y;

    public Camera _MainCamera;

    public Transform m_kTRS;

    public Transform m_kMainCameraTRS;

    //
    // Properties
    //
    public Camera mainCamera
    {
        get
        {
            return this._MainCamera;
        }
        set
        {
            this._MainCamera = value;
            this.m_kMainCameraTRS = this._MainCamera.transform;
        }
    }

    //
    // Methods
    //
    private void Start()
    {
        if (this._MainCamera == null)
        {
            this._MainCamera = GameTools.mainCamera;
        }
        if (this._MainCamera == null)
        {
            GameObject.Destroy(this);
            return;
        }
        this.m_kMainCameraTRS = this._MainCamera.transform;
        this.m_kTRS = base.transform;
    }

    private void Update()
    {
        if (this.m_kMainCameraTRS == null)
        {
            return;
        }
        switch (this._BillboardType)
        {
            case IzBillboard.BILLBOARD_TYPE.PERPENDICUAL_VIEW_ALL:
                {
                    Vector3 vector = -this.m_kMainCameraTRS.forward;
                    Vector3 vector2 = this.m_kTRS.position + vector * 10;
                    this.m_kTRS.LookAt(vector2);
                    break;
                }
            case IzBillboard.BILLBOARD_TYPE.PERPENDICUAL_VIEW_X:
                {
                    Vector3 vector = -this.m_kMainCameraTRS.forward;
                    vector.x = 0;
                    this.m_kTRS.forward = vector;
                    break;
                }
            case IzBillboard.BILLBOARD_TYPE.PERPENDICUAL_VIEW_Y:
                {
                    Vector3 vector = -this.m_kMainCameraTRS.forward;
                    vector.y = 0;
                    this.m_kTRS.forward = vector;
                    break;
                }
            case IzBillboard.BILLBOARD_TYPE.PERPENDICUAL_VIEW_Z:
                {
                    Vector3 vector = -this.m_kMainCameraTRS.forward;
                    vector.z = 0;
                    this.m_kTRS.forward = vector;
                    break;
                }
            case IzBillboard.BILLBOARD_TYPE.INV_PERPENDICUAL_VIEW_ALL:
                {
                    Vector3 vector = this.m_kMainCameraTRS.forward;
                    Vector3 vector2 = this.m_kTRS.position + vector * 10;
                    this.m_kTRS.LookAt(vector2);
                    break;
                }
            case IzBillboard.BILLBOARD_TYPE.INV_PERPENDICUAL_VIEW_X:
                {
                    Vector3 vector = this.m_kMainCameraTRS.forward;
                    vector.x = 0;
                    this.m_kTRS.forward = vector;
                    break;
                }
            case IzBillboard.BILLBOARD_TYPE.INV_PERPENDICUAL_VIEW_Y:
                {
                    Vector3 vector = this.m_kMainCameraTRS.forward;
                    vector.y = 0;
                    this.m_kTRS.forward = vector;
                    break;
                }
            case IzBillboard.BILLBOARD_TYPE.INV_PERPENDICUAL_VIEW_Z:
                {
                    Vector3 vector = -this.m_kMainCameraTRS.forward;
                    vector.z = 0;
                    this.m_kTRS.forward = vector;
                    break;
                }
            case IzBillboard.BILLBOARD_TYPE.FACE_EYE_ALL:
                this.m_kTRS.LookAt(this.m_kMainCameraTRS.position);
                break;
            case IzBillboard.BILLBOARD_TYPE.FACE_EYE_X:
                {
                    Vector3 vector2 = this.m_kMainCameraTRS.position;
                    vector2.x = this.m_kTRS.position.x;
                    this.m_kTRS.LookAt(vector2);
                    break;
                }
            case IzBillboard.BILLBOARD_TYPE.FACE_EYE_Y:
                {
                    Vector3 vector2 = this.m_kMainCameraTRS.position;
                    vector2.y = this.m_kTRS.position.y;
                    this.m_kTRS.LookAt(vector2);
                    break;
                }
            case IzBillboard.BILLBOARD_TYPE.FACE_EYE_Z:
                {
                    Vector3 vector2 = this.m_kMainCameraTRS.position;
                    vector2.z = this.m_kTRS.position.z;
                    this.m_kTRS.LookAt(vector2);
                    break;
                }
            case IzBillboard.BILLBOARD_TYPE.INV_FACE_EYE_ALL:
                this.m_kTRS.LookAt(this.m_kMainCameraTRS.position);
                this.m_kTRS.forward = -this.m_kTRS.forward;
                break;
            case IzBillboard.BILLBOARD_TYPE.INV_FACE_EYE_X:
                {
                    Vector3 vector2 = this.m_kMainCameraTRS.position;
                    vector2.x = this.m_kTRS.position.x;
                    this.m_kTRS.LookAt(vector2);
                    this.m_kTRS.forward = -this.m_kTRS.forward;
                    break;
                }
            case IzBillboard.BILLBOARD_TYPE.INV_FACE_EYE_Y:
                {
                    Vector3 vector2 = this.m_kMainCameraTRS.position;
                    vector2.y = this.m_kTRS.position.y;
                    this.m_kTRS.LookAt(vector2);
                    this.m_kTRS.forward = -this.m_kTRS.forward;
                    break;
                }
            case IzBillboard.BILLBOARD_TYPE.INV_FACE_EYE_Z:
                {
                    Vector3 vector2 = this.m_kMainCameraTRS.position;
                    vector2.z = this.m_kTRS.position.z;
                    this.m_kTRS.LookAt(vector2);
                    this.m_kTRS.forward = -this.m_kTRS.forward;
                    break;
                }
            case IzBillboard.BILLBOARD_TYPE.FACE_Y:
                {
                    Vector3 vector2 = new Vector3(-90, 0, 0) + this.m_kMainCameraTRS.rotation.eulerAngles;
                    this.m_kTRS.rotation = Quaternion.Euler(vector2);
                    break;
                }
        }
    }

    //
    // Nested Types
    //
    public enum BILLBOARD_TYPE
    {
        PERPENDICUAL_VIEW_ALL,
        PERPENDICUAL_VIEW_X,
        PERPENDICUAL_VIEW_Y,
        PERPENDICUAL_VIEW_Z,
        INV_PERPENDICUAL_VIEW_ALL,
        INV_PERPENDICUAL_VIEW_X,
        INV_PERPENDICUAL_VIEW_Y,
        INV_PERPENDICUAL_VIEW_Z,
        FACE_EYE_ALL,
        FACE_EYE_X,
        FACE_EYE_Y,
        FACE_EYE_Z,
        INV_FACE_EYE_ALL,
        INV_FACE_EYE_X,
        INV_FACE_EYE_Y,
        INV_FACE_EYE_Z,
        FACE_X,
        FACE_Y,
        FACE_Z
    }
}
