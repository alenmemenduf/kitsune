using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;            //Radius of the circle in which we want the camera to
    [Range(0,360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public Material redFovMaterial;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();
    public float meshResolution;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;
    Material whiteFovMaterial;
    


    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        whiteFovMaterial = viewMeshFilter.gameObject.GetComponent<MeshRenderer>().material;
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
        if(visibleTargets.Count != 0)
        {

            viewMeshFilter.gameObject.GetComponent<MeshRenderer>().material = redFovMaterial;

        }
        else
        {
            viewMeshFilter.gameObject.GetComponent<MeshRenderer>().material = whiteFovMaterial;
        }
    }
    public Vector2 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees -= transform.eulerAngles.z;
        }
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void DrawFieldOfView()
    {
        int stepCount =Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();

        for(int i = 0; i <= stepCount; i++)
        {
            float angle = (transform.eulerAngles.z - viewAngle/2 + stepAngleSize * i);
            ViewCastInfo newViewCast = ViewCast(-angle);
            viewPoints.Add(newViewCast.point);
        }

        int vertexCount = viewPoints.Count + 1;

        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for(int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();

    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector2 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);

        if (hit)
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false,(Vector2) transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }
    public void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);
        for(int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
         
            Vector2 dirToTarget = (target.position - transform.position).normalized;
            if(Vector3.Angle (transform.up, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector2.Distance(transform.position, target.position);

                if(!Physics2D.Raycast(transform.position, dirToTarget,distToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    target.GetComponent<Player>().isDead = true;
                    gameObject.GetComponent<EnemyCamera>().enabled = false;
                }
 
            }
        }
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector2 point;
        public float dist;
        public float angle;

        public ViewCastInfo(bool _hit, Vector2 _point, float _dist, float _angle)
        {
            hit = _hit;
            point = _point;
            dist = _dist;
            angle = _angle;
        }
    }
}
