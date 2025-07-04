using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSelector : MonoBehaviour
{
    public float TransitionSpeed = 5f;
    /// the threshold distance at which the marker will stop moving
    public float MinimalTransitionDistance = 0.01f;

    protected RectTransform _rectTransform;
    protected Image image;
    protected GameObject _currentSelection;
    protected Vector3 _originPosition;
    protected Vector3 _originLocalScale;
    protected Vector3 _originSizeDelta;
    protected float _originTime;
    protected bool _originIsNull = true;
    protected float _deltaTime;
    protected bool nullSelected;

    /// <summary>
    /// On Start, we get the associated rect transform
    /// </summary>
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    /// <summary>
    /// On Update, we get the current selected object, and we move the marker to it if necessary
    /// </summary>
    void Update()
    {
        _currentSelection = EventSystem.current.currentSelectedGameObject;
        if (_currentSelection == null)
        {
            image.enabled = false;
            if (!nullSelected)
            {
                ItemActionController.Instance.currentSelectedSlot = null;
                ItemActionController.Instance.ShowChoices(false);
                nullSelected = true;
                EventSystem.current.SetSelectedGameObject(null);
            }

            return;
        }
        else
        {
            image.enabled = true;
            nullSelected = false;
        }


        if (_currentSelection.gameObject.GetComponent<ItemSlot>() == null)
        {
            image.enabled = false;

            if (!nullSelected)
            {
                nullSelected = true;

                if (!_currentSelection.gameObject.name.Contains("Button"))
                {
                    ItemActionController.Instance.currentSelectedSlot = null;
                }
            }

            return;
        }

        if (Vector3.Distance(transform.position, _currentSelection.transform.position) > MinimalTransitionDistance)
        {
            if (_originIsNull)
            {
                _originIsNull = false;
                _originPosition = transform.position;
                _originLocalScale = _rectTransform.localScale;
                _originSizeDelta = _rectTransform.sizeDelta;
                _originTime = Time.unscaledTime;
            }
            _deltaTime = (Time.unscaledTime - _originTime) * TransitionSpeed;
            transform.position = Vector3.Lerp(_originPosition, _currentSelection.transform.position, _deltaTime);
            _rectTransform.localScale = Vector3.Lerp(_originLocalScale, _currentSelection.GetComponent<RectTransform>().localScale, _deltaTime);
        }
        else
        {
            _originIsNull = true;
        }
    }
}
