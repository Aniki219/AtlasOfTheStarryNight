using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Malee.Hive.Components
{

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteMask))]
    public sealed class ScreenSpriteMask : MonoBehaviour
    {

        private const float PIXELS_PER_UNIT = 100;

        private static readonly CameraType cameraMask = CameraType.Game | CameraType.SceneView | CameraType.VR;

        enum Depth
        {
            None = 0,
            Depth16Bit = 16,
            Depth24Bit = 24
        }

        [SerializeField, Delayed, Tooltip("Size of the mask texture. Higher creates a smoother Mask")]
        private int textureSize = 256;

        [SerializeField, Tooltip("Determines what layers are rendered into the Mask")]
        private LayerMask cullingMask = -1;

        [SerializeField, Tooltip("Depth buffer bits. Masking is generally only interested in the shape, so default is None")]
        private Depth depthBuffer = Depth.None;

        [SerializeField, Tooltip("Use Graphics.CopyTexture when rendering screen into sprite, may not work on some Graphics APIs!")]
        private bool useCopyTexture = false;

        private Sprite _sprite;
        public Sprite sprite
        {

            get { return _sprite; }
        }

        private SpriteMask mask;
        private Texture2D texture;
        private Camera maskCamera;
        private bool rendering;

        private void Awake()
        {

            mask = GetComponent<SpriteMask>();
        }

        private void OnEnable()
        {

            rendering = false;

            Camera.onPreCull -= OnRender;
            Camera.onPreCull += OnRender;
        }

        private void OnDisable()
        {

            Dispose();
        }

        private void OnDestroy()
        {

            Dispose();
        }

        private void OnRender(Camera camera)
        {

            if (!enabled || !camera || rendering)
            {

                return;
            }

            rendering = true;

            //clamp the texture size, does a mask need to be more than 4096px? I think not

            int size = Mathf.Clamp(textureSize, 1, 4096);

            CreateObjects(size);

            if (camera != maskCamera && (camera.cameraType & cameraMask) != 0)
            {

                RenderTexture maskTexture = RenderTexture.GetTemporary(size, size, (int)depthBuffer, RenderTextureFormat.ARGB32);

                maskCamera.CopyFrom(camera);
                maskCamera.enabled = false;
                maskCamera.clearFlags = CameraClearFlags.Color;
                maskCamera.cullingMask = cullingMask;
                maskCamera.backgroundColor = Color.clear;
                maskCamera.targetTexture = maskTexture;
                maskCamera.Render();

                if (useCopyTexture)
                {

                    Graphics.CopyTexture(maskTexture, texture);
                }
                else
                {

                    RenderTexture active = RenderTexture.active;
                    RenderTexture.active = maskTexture;

                    texture.ReadPixels(new Rect(0, 0, size, size), 0, 0);
                    texture.Apply();

                    RenderTexture.active = active;
                }

                RenderTexture.ReleaseTemporary(maskTexture);

                //place us in screen space

                float frustumHeight = size / PIXELS_PER_UNIT;
                float projectedDistance = frustumHeight * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

                transform.position = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, projectedDistance));
                transform.rotation = camera.transform.rotation;
            }

            rendering = false;
        }

        private void CreateObjects(int size)
        {

            if (!maskCamera)
            {

                maskCamera = new GameObject("Camera" + name, typeof(Camera)).GetComponent<Camera>();
                maskCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
                maskCamera.enabled = false;
            }

            if (!texture)
            {

                texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
                texture.hideFlags = HideFlags.HideAndDontSave;

                _sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), PIXELS_PER_UNIT);
                _sprite.hideFlags = HideFlags.HideAndDontSave;
            }
            else if (texture.width != size || texture.height != size)
            {

                //rezize texture if changed, we also have to create the Sprite again because its rect has changed

                if (_sprite)
                {

                    DestroyImmediate(_sprite);
                }

                texture.Resize(size, size, TextureFormat.ARGB32, false);
                texture.Apply();

                _sprite = Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f), PIXELS_PER_UNIT);
                _sprite.hideFlags = HideFlags.HideAndDontSave;
            }

            //Apply mask if it's null, allow user to manually set mask if they wish

            if (!mask.sprite)
            {

                mask.sprite = _sprite;
            }
        }

        private void Dispose()
        {

            Camera.onPreCull -= OnRender;

            if (_sprite)
            {

                mask.sprite = null;

                DestroyImmediate(_sprite);
                _sprite = null;
            }

            if (texture)
            {

                DestroyImmediate(texture);
                texture = null;
            }

            if (maskCamera)
            {

                DestroyImmediate(maskCamera.gameObject);
                maskCamera = null;
            }
        }
    }
}