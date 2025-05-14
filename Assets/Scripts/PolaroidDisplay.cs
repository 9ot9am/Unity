using System;
using DG.Tweening;
using UnityEngine;


    public class PolaroidDisplay : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Renderer photoRenderer; // Assign the Quad that displays the photo

        private void Start()
        {
            var rightController = GameObject.FindGameObjectWithTag("RightController");

            if (!rightController) return;
            transform.DOMove(rightController.transform.position, 1f)
                .SetEase(Ease.InCubic);
        }

        public void SetPhoto(Texture2D newPhotoTexture)
        {
            if (newPhotoTexture == null)
            {
                Debug.LogWarning("Tried to set a null photo texture.", this);
                return;
            }
            photoRenderer.material.mainTexture = newPhotoTexture;

                // Optional: Adjust aspect ratio of the quad to match the texture
                // This keeps the image from being stretched.
                // Ensure the quad is initially 1x1 for this to work easily.
                if (newPhotoTexture.width != 0 && newPhotoTexture.height != 0)
                {
                    float aspectRatio = (float)newPhotoTexture.width / newPhotoTexture.height;
                    Vector3 newScale = photoRenderer.transform.localScale;

                    if (aspectRatio >= 1) // Landscape or square
                    {
                        newScale.x = photoRenderer.transform.parent ? 1f : 1f; // Assuming parent provides overall scale
                        newScale.y = newScale.x / aspectRatio;
                    }
                    else // Portrait
                    {
                        newScale.y = photoRenderer.transform.parent ? 1f : 1f;
                        newScale.x = newScale.y * aspectRatio;
                    }
                    // This assumes the photoRenderer quad itself is scaled.
                    // If the photoRenderer is a child of a border, you might scale its localScale.
                    // For a typical polaroid, the inner picture is square-ish, so this might not be strictly needed
                    // or you might want to scale the parent "PolaroidBorder" object proportionally instead.
                    // photoRenderer.transform.localScale = newScale; // Uncomment and adapt if needed
                }
        }
    }
