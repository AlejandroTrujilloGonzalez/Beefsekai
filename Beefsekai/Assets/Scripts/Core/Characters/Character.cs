using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Character
{

    public string characterName;
    [HideInInspector]public RectTransform root;

    public bool isMultiLayerCharacter { get { return renderers.renderer == null; } }
    public bool enabled { get { return root.gameObject.activeInHierarchy; } set { root.gameObject.SetActive(value); } }

    public Vector2 anchorPadding { get { return root.anchorMax - root.anchorMin; } }

    DialogueSyst dialogue;


    public void Say(string speech, bool add = false)
    {
        if (!enabled)
        {
            enabled = true;
        }

        if (!add)
        {
            dialogue.Say(speech, characterName);
        }
        else
        {
            dialogue.SayAdd(speech, characterName);
        }

    }

    //Crear un nuevo personaje//////////////////////////////////////////////////////////////////////////////////
    public Character(string _name, bool enableOnStart = true)//Esto da una alerta y no entiendo el motivo
    {
        CharacterManager characterManager = CharacterManager.instance;
        GameObject prefab = Resources.Load("Characters/Character[" + _name + "]") as GameObject;//Importante, por el momento, el nombre entre corchetes del prefab debe coincidir con el que llamemos cuando creamos un nuevo personaje
        Debug.Log(prefab.name);
        GameObject go = GameObject.Instantiate(prefab, characterManager.characterPanel);

        root = go.GetComponent<RectTransform>();
        characterName = _name;

        renderers.renderer = go.GetComponentInChildren<RawImage>();

        if (isMultiLayerCharacter)
        {
            renderers.bodyRender = go.transform.Find("BodyLayer").GetComponentInChildren<Image>();
            renderers.expressionRenderer = go.transform.Find("ExpressionLayer").GetComponentInChildren<Image>();
            renderers.allBodyRenderers.Add(renderers.bodyRender);
            renderers.allExpressionRenderers.Add(renderers.expressionRenderer);
        }


        dialogue = DialogueSyst.instance;
        enabled = enableOnStart;
    }


    [System.Serializable]
    public class Renderers
    {
        public RawImage renderer;

        public Image bodyRender;
        public Image expressionRenderer;

        public List<Image> allBodyRenderers = new List<Image>();
        public List<Image> allExpressionRenderers = new List<Image>();

    }

    public Renderers renderers = new Renderers();


    //M�todos y atributos encargados del movimiento de los personajes//////////////////////////////////////////////////////////////////////////////
    Vector2 targetPosition;
    Coroutine moving;
    bool isMoving { get { return moving != null; } }

    public void MoveTo(Vector2 target, float speed, bool smooth = true)
    {
        StopMoving();
        moving = CharacterManager.instance.StartCoroutine(Moving(target, speed, smooth));
    }

    public void StopMoving(bool arriveAtTargetPositionImmediately = false)
    {
        if (isMoving)
        {
            CharacterManager.instance.StopCoroutine(moving);
            if (arriveAtTargetPositionImmediately)
            {
                SetPosition(targetPosition);
            }
        }

        moving = null;
    }

    public void SetPosition(Vector2 target)
    {
        Vector2 padding = anchorPadding;

        float maxX = 1f - padding.x;
        float maxY = 1f - padding.y;

        Vector2 minAnchorTarget = new Vector2(maxX * targetPosition.x, maxY * targetPosition.y);
        root.anchorMin = minAnchorTarget;
        root.anchorMax = root.anchorMin + padding;
    }

    IEnumerator Moving(Vector2 target, float speed, bool smooth)
    {
        targetPosition = target;

        Vector2 padding = anchorPadding;

        float maxX = 1f - padding.x;
        float maxY = 1f - padding.y;

        Vector2 minAnchorTarget = new Vector2(maxX * targetPosition.x, maxY * targetPosition.y);
        speed *= Time.deltaTime;

        while (root.anchorMin != minAnchorTarget)
        {
            root.anchorMin = (!smooth) ? Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed) : Vector2.Lerp(root.anchorMin, minAnchorTarget, speed);
            root.anchorMax = root.anchorMin + padding;
            yield return new WaitForEndOfFrame();
        }

        StopMoving();
    }



    //M�todos y atributos para las transiciones entre im�genes, todo lo relacionado con el multicapa//////////////////////////////////////////////////////////////////////////////////////////////////
    public Sprite GetSprite(int index = 0)
    {

        Sprite sprite = Resources.Load<Sprite>("Art/CharactersImage/moneco_Bartowolo");//Esto es otra forma de llamar a los sprites, en vez de por su index dentro del spriteseet por su nombre, nombre que tambi�n est� en una clase del characmanager, es opcional hacelro asi o de la otra forma
        return sprite;

        //Sprite[] sprites = Resources.LoadAll<Sprite>("Art/CharactersImage/" + characterName);
        //return sprites[index];
    }

    public void SetBody(int index)
    {
        renderers.bodyRender.sprite = GetSprite(index);
    }
    public void SetBody(Sprite sprite)
    {
        renderers.bodyRender.sprite = sprite;
    }

    public void SetExpression(int index)
    {
        renderers.expressionRenderer.sprite = GetSprite(index);
    }
    public void SetExpression(Sprite sprite)
    {
        renderers.expressionRenderer.sprite = sprite;
    }

    //Transition Body////////////
    bool isTransitioningBody { get { return transitioningBody != null; } }
    Coroutine transitioningBody = null;

    public void TransitionBody(Sprite sprite, float speed, bool smooth)
    {
        if (renderers.bodyRender.sprite == sprite)
        {
            return;
        }
        StopTransitioningBody();
        transitioningBody = CharacterManager.instance.StartCoroutine(TransitioningBody(sprite, speed, smooth));
    }

    private void StopTransitioningBody()
    {
        if (isTransitioningBody)
        {
            CharacterManager.instance.StopCoroutine(transitioningBody);
        }

        transitioningBody = null;
    }


    public IEnumerator TransitioningBody(Sprite sprite, float speed, bool smooth)
    {
        for (int i = 0; i < renderers.allBodyRenderers.Count; i++)
        {
            Image image = renderers.allBodyRenderers[i];
            if (image.sprite == sprite)
            {
                renderers.bodyRender = image;
                break;
            }
        }

        if (renderers.bodyRender.sprite != sprite)
        {
            Image image = GameObject.Instantiate(renderers.bodyRender.gameObject, renderers.bodyRender.transform.parent).GetComponent<Image>();
            renderers.allBodyRenderers.Add(image);
            renderers.bodyRender = image;
            image.color = GlobalFunctions.SetAlpha(image.color, 0f);
            image.sprite = sprite;
        }

        while (GlobalFunctions.TransitionImages(ref renderers.bodyRender, ref renderers.allBodyRenderers, speed,smooth))
        {
            yield return new WaitForEndOfFrame();
        }

        StopTransitioningBody();

    }

    //Transition Expression///////////
    bool isTransitioningExpression { get { return transitioningExpression != null; } }
    Coroutine transitioningExpression = null;

    public void TransitionExpression(Sprite sprite, float speed, bool smooth)
    {
        if (renderers.expressionRenderer.sprite == sprite)
        {
            return;
        }
        StopTransitioningExpression();
        transitioningExpression = CharacterManager.instance.StartCoroutine(TransitioningExpression(sprite, speed, smooth));
    }

    private void StopTransitioningExpression()
    {
        if (isTransitioningExpression)
        {
            CharacterManager.instance.StopCoroutine(transitioningExpression);
        }

        transitioningExpression = null;
    }


    public IEnumerator TransitioningExpression(Sprite sprite, float speed, bool smooth)
    {
        for (int i = 0; i < renderers.allExpressionRenderers.Count; i++)
        {
            Image image = renderers.allExpressionRenderers[i];
            if (image.sprite == sprite)
            {
                renderers.expressionRenderer = image;
                break;
            }
        }

        if (renderers.expressionRenderer.sprite != sprite)
        {
            Image image = GameObject.Instantiate(renderers.expressionRenderer.gameObject, renderers.expressionRenderer.transform.parent).GetComponent<Image>();
            renderers.allExpressionRenderers.Add(image);
            renderers.expressionRenderer = image;
            image.color = GlobalFunctions.SetAlpha(image.color, 0f);
            image.sprite = sprite;
        }

        while (GlobalFunctions.TransitionImages(ref renderers.expressionRenderer, ref renderers.allExpressionRenderers, speed, smooth))
        {
            yield return new WaitForEndOfFrame();
        }

        StopTransitioningExpression();

    }

}