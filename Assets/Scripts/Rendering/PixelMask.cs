/*
Copyright © 2011, Stephan Krohn
All rights reserved.
krohn.stephan@googlemail.com
*/

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(GUITexture))]
public class PixelMask : MonoBehaviour
{

	Texture2D texture;

	public static PixelMask Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = GameObject.Instantiate(Resources.Load("PixelMask", typeof(GameObject))) as GameObject;
				instance = go.GetComponent<PixelMask>();
			}
			return instance;
		}
	}
	static PixelMask instance;

	void Awake()
	{
		instance = this;
	}

	public void CreateMask(Quad frame)
	{
		Logger.Log("Erstelle Pixelmaske.");

		// create a new texture that is fullscreen sized
		if (texture != null)
			Object.DestroyImmediate(texture);

		texture = new Texture2D(Screen.width, Screen.height);

		// get pixel array (better performance)
		Color[] pixels = texture.GetPixels();

		// set all pixels to black.
		// set pixels that are inside the frame to transparent
		for (int x = 0; x < texture.width; x++)
		{
			for (int y = 0; y < texture.height; y++)
			{
				int pos = (y * texture.width) + x;

				pixels[pos] = Color.black;

				//				pixels[pos].a = 1;
				Vector2 position = new Vector2((float)x / Screen.width, (float)y / Screen.height);
				if (frame.Contains(position))
				{
					pixels[pos].a = 0;
				}
			}
		}

		// upload texture to vram
		texture.SetPixels(pixels);
		texture.Apply();

		// adjust the texture size and assign texture to GUITexture component
		guiTexture.pixelInset = new Rect(0, 0, texture.width, texture.height);
		guiTexture.texture = texture;

		gameObject.SetActive(true);
	}

	void OnDestroy()
	{
		// destroy texture (get's leaked in editor otherwise)
		Object.DestroyImmediate(texture);
	}
}
