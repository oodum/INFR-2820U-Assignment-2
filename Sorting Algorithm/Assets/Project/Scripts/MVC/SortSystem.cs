using System;
using Project.Scripts.Algorithms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Project.Scripts.MVC {
    public class SortSystem : MonoBehaviour {
        [HideInInspector] public SortingStrategy strategy;
        [HideInInspector] public int length;
        
        [Header("MVC")]
        [SerializeField] SortView view;
        SortController controller;

        [Header("Element Control")]
        [SerializeField] Slider elementSlider;
        [SerializeField] TextMeshProUGUI elementText;
        
        [Header("Other Control")]
        [SerializeField] Button randomizeButton;
        [SerializeField] TMP_Dropdown strategyDropdown;
        
        [Header("Strategy Control")]
        SortingStrategy[] strategies;
        
        void Awake() {
            controller = new SortController(view);
            elementSlider.onValueChanged.AddListener(SetLength);
            strategyDropdown.onValueChanged.AddListener(SetStrategy);

            strategies = new SortingStrategy[] {
                new BubbleSort(),
                new InsertionSort(),
                new SelectionSort(),
                new MergeSort(),
            };
        }
        void Start() {
            SetLength(elementSlider.value);
            Initialize();
        }
        void Update() {
            if (!controller.Step()) {
                SetButtons(true);
            }
        }
        
        void SetLength(float elements) {
            length = Mathf.FloorToInt(elements);
            elementText.text = length.ToString();
            controller.SetLength(length);
        }

        public void Randomize() {
            controller.Randomize();
        }

        public void Play() {
            bool playing = controller.Play();
            SetButtons(!playing);
        }
        
        void SetButtons(bool enabled) {
            elementSlider.interactable = enabled;
            randomizeButton.interactable = enabled;
            strategyDropdown.interactable = enabled;
        }
        
        public void SetStrategy(int index) {
            strategy = strategies[index];
            controller.SetStrategy(strategy);
        }

        void Initialize() {
            SetStrategy(0);
            controller.SetStrategy(strategy);
            Randomize();
            controller.SetLength(view.length);
        }
    }
 }
