using System.Collections;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ShockwaveController : MonoBehaviour
{
	const string RADIUS_LOOP_PROP = "_IsRadiusLoopActive";
	const string RADIUS_STATIC_PROP = "_RadiusStaticValue";
	
	MeshRenderer _meshRenderer;
	[SerializeField, Required] Material _shockwaveMaterial;
	[SerializeField] float _duration = 1f;

	void Awake()
	{
		Initialize();
	}

	[Button]
	void Initialize()
	{
		_meshRenderer = GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();
		if (_shockwaveMaterial == null)
		{			
			Debug.LogError("Shockwave material is not assigned");
			return;
		}
		
		_meshRenderer.material = _shockwaveMaterial;
		_shockwaveMaterial.SetInt(RADIUS_LOOP_PROP, 0);
		_shockwaveMaterial.SetFloat(RADIUS_STATIC_PROP, 0);
	}

	[Button]
	public void PlayShockwave()
	{
		StartCoroutine(ShockwaveOneShot());
	}

	IEnumerator ShockwaveOneShot()
	{
		_shockwaveMaterial.SetInt(RADIUS_LOOP_PROP, 0);
		_shockwaveMaterial.SetFloat(RADIUS_STATIC_PROP, 0);
		
		float elapsedTime = 0f;
		while (elapsedTime < _duration)
		{
			elapsedTime += Time.deltaTime;
			_shockwaveMaterial.SetFloat(RADIUS_STATIC_PROP, elapsedTime / _duration);
			yield return null;
		}
		
		_shockwaveMaterial.SetFloat(RADIUS_STATIC_PROP, 0);
	}
}
