using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARFeatheredPlaneMeshVisualizer : MonoBehaviour
{
	[SerializeField]
	private ARPlaneMeshVisualizer _planeMeshVisualizer;
	[SerializeField]
	private ARPlane _plane;
	[SerializeField]
	private Renderer _featheredPlaneRenderer;
	[Tooltip("The width of the texture feathering (in world units).")]
	[SerializeField]
	private float _featheringWidth = 0.2f;

	private static List<Vector3> _featheringUVs = new List<Vector3>();
	private static List<Vector3> _vertices = new List<Vector3>();
	private Material _cacheMaterial;

	public float FeatheringWidth
	{
		get { return _featheringWidth; }
		set { _featheringWidth = value; }
	}

	private void Awake()
	{
		_cacheMaterial = _featheredPlaneRenderer.material;
	}

	private void OnDestroy()
	{
		if(_cacheMaterial != null )
		{
			DestroyImmediate(_cacheMaterial);
		}
		_cacheMaterial = null;
	}

	void OnEnable()
	{
		_plane.boundaryChanged += OnARPlaneBoundaryUpdated;
	}

	void OnDisable()
	{
		_plane.boundaryChanged -= OnARPlaneBoundaryUpdated;
	}

	void OnARPlaneBoundaryUpdated(ARPlaneBoundaryChangedEventArgs eventArgs)
	{
		GenerateBoundaryUVs(_planeMeshVisualizer.mesh);
	}

	void GenerateBoundaryUVs(Mesh mesh)
	{
		int vertexCount = mesh.vertexCount;

		// Reuse the list of UVs
		_featheringUVs.Clear();
		if (_featheringUVs.Capacity < vertexCount) { _featheringUVs.Capacity = vertexCount; }

		mesh.GetVertices(_vertices);

		Vector3 centerInPlaneSpace = _vertices[_vertices.Count - 1];
		Vector3 uv = new Vector3(0, 0, 0);
		float shortestUVMapping = float.MaxValue;

		// Assume the last vertex is the center vertex.
		for (int i = 0; i < vertexCount - 1; i++)
		{
			float vertexDist = Vector3.Distance(_vertices[i], centerInPlaneSpace);
			float uvMapping = vertexDist / Mathf.Max(vertexDist - _featheringWidth, 0.001f);
			uv.x = uvMapping;

			if (shortestUVMapping > uvMapping) { shortestUVMapping = uvMapping; }

			_featheringUVs.Add(uv);
		}

		_cacheMaterial.SetFloat("_ShortestUVMapping", shortestUVMapping);

		// Add the center vertex UV
		uv.Set(0, 0, 0);
		_featheringUVs.Add(uv);

		mesh.SetUVs(1, _featheringUVs);
		mesh.UploadMeshData(false);
	}
}
