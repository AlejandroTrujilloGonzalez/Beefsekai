using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGController : MonoBehaviour
{

    public static BGController instance;

    public Layer background = new Layer();
    //public Layer foreground = new Layer();


    private void Awake()
    {
        instance = this;
    }

    [System.Serializable]
    public class Layer
    {
        public GameObject root;
        public GameObject newImageObjectReference;
        public RawImage activeImage;
        public List<RawImage> allImages = new List<RawImage>();


        public void SetTexture(Texture texture)
        {
            if (texture != null)
            {
                if (activeImage == null)
                {
                    CreateNewActiveImage();
                }
                activeImage.texture = texture;
                activeImage.color = GlobalFunctions.SetAlpha(activeImage.color, 1f);
            }
            else
            {
                if (activeImage != null)
                {
                    allImages.Remove(activeImage);
                    GameObject.Destroy(activeImage.gameObject);
                    activeImage = null;
                }
            }

        }

        public void TransitionToTexture(Texture texture, float speed = 2f, bool smooth = false)
        {

            if (activeImage != null && activeImage.texture == texture)
            {
                return;
            }

            StopTransitioning();
            transitioning = BGController.instance.StartCoroutine(Transitioning(texture,speed,smooth));
        }

        private void StopTransitioning()
        {
            if (isTransitioning)
            {
                BGController.instance.StopCoroutine(transitioning);
            }

            transitioning = null;

        }

        public bool isTransitioning { get { return transitioning != null; } }
        Coroutine transitioning = null;
        IEnumerator Transitioning(Texture texture, float speed, bool smooth)
        {
            if (texture != null)
            {
                for (int i = 0; i < allImages.Count; i++)
                {
                    RawImage image = allImages[i];
                    if (image.texture == texture)
                    {
                        activeImage = image;
                        break;
                    }
                }

                if (activeImage == null || activeImage.texture != texture)
                {
                    CreateNewActiveImage();
                    activeImage.texture = texture;
                    activeImage.color = GlobalFunctions.SetAlpha(activeImage.color, 0f);
                }
            }
            else
            {
                activeImage = null;
            }


            while (GlobalFunctions.TransitionRawImages(ref activeImage, ref allImages, speed,smooth))
            {
                yield return new WaitForEndOfFrame();
            }

            StopTransitioning();

        }

        public void CreateNewActiveImage()
        {
            Debug.Log("Test");
            GameObject go = Instantiate(newImageObjectReference, root.transform) as GameObject;
            go.SetActive(true);
            RawImage raw = go.GetComponent<RawImage>();
            raw = activeImage;
            allImages.Add(raw);
        }



    }



}
