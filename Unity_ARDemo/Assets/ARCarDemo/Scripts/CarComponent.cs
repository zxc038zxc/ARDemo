using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarComponent : MonoBehaviour
{
	[SerializeField]
	private List<Animation> _doorList;
	[SerializeField]
	private Animation _trunk;
	[SerializeField]
	private Animation _wheel;


	private List<Material> _cacheMaterials = new List<Material>();
	private bool _currentDoorIsOpen = false;

	private void Awake()
	{
		MeshRenderer[] meshs = GetComponentsInChildren<MeshRenderer>(true);
		foreach(var mesh in meshs)
		{
			Material[] materials = mesh.GetComponent<Renderer>().materials;
			foreach (var mat in materials)
			{
				if(mat.name.StartsWith("BreaksRedPain"))
				{
					_cacheMaterials.Add(mat);
				}
			}
		}
	}

	public void OpenOrCloseDoor()
	{
		_currentDoorIsOpen = !_currentDoorIsOpen;
		for (int i = 0; i < _doorList.Count; i++)
		{
			_doorList[i].Play(_currentDoorIsOpen ? "OpenDoor" : "CloseDoor");
			_trunk.Play(_currentDoorIsOpen ? "CloseDoor" : "Idle");
			_wheel.Play(_currentDoorIsOpen ? "Run" : "Idle");
		}
	}

	public void ChangeColor(Color color)
	{
		foreach(var mat in _cacheMaterials)
		{
			mat.SetColor("_BaseColor", color);
		}
	}
}
