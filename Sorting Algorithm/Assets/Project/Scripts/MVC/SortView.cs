using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Algorithms;
using UnityEngine;
namespace Project.Scripts.MVC {
    public class SortView : MonoBehaviour {
        [SerializeField] RectTransform barPrefab;
        [SerializeField] Transform barParent;
        [SerializeField] AudioSource[] blips;
        List<RectTransform> bars = new();
        public int length = 2;
        int[] previous;
        int blipIndex = 0;
        public void SetArray(int[] a) {
            previous = a;
            length = a.Length;
            for (int i = 0; i < a.Length; i++) {
                if (i < bars.Count) {
                    bars[i].sizeDelta = new Vector2(15, a[i] * 5);
                }
                else {
                    var bar = Instantiate(barPrefab, barParent);
                    bar.sizeDelta = new Vector2(15, a[i] * 5);
                    bars.Add(bar);
                }
            }
            while(bars.Count > a.Length) {
                var lastBar = bars[^1];
                bars.RemoveAt(bars.Count - 1);
                Destroy(lastBar.gameObject);
            }
        }
        public void UpdateView(int[] a) {
            int largest = 0;
            for (int i = 0; i < a.Length; i++) {
                bars[i].sizeDelta = new Vector2(15, a[i] * 5);
                if (previous[i] != a[i]) {
                    largest = Mathf.Max(largest, a[i]);
                }
            }
            var blip = blips[blipIndex];
            blip.pitch = 4 * (largest / 100f) - 2;
            blip.Play();
            blipIndex = (blipIndex + 1) % blips.Length;
            previous = a;
        }
    }

    public class SortController {
        SortView view;
        SortModel model;
        bool playing = false;
        public SortController(SortView view) {
            this.view = view;
            model = new SortModel();
        }

        public bool Step() {
            if (!playing) return false;
            var step = model.Step();
            view.UpdateView(step.a);
            if (!step.completed) {
                return true;
            }
            playing = false;
            return false;
        }

        public void SetStrategy(SortingStrategy strategy) {
            model.SetStrategy(strategy);
        }

        public void Randomize() {
            model.Randomize();
            view.SetArray(model.RetrieveArrayByLength(model.Length));
        }
        public void SetLength(int length) {
            model.Length = length;
            view.SetArray(model.RetrieveArrayByLength(length));
            model.SetLength(length);
        }

        public bool Play() {
            playing = !playing;
            return playing;
        }
    }

    public class SortModel {
        int[] a = new int[100];
        public int Length = 2;
        public int Steps = 0;
        public SortingStrategy strategy;

        public (bool completed, int[] a) Step() {
            strategy.Step();
            a = strategy.A;
            return (strategy.completed, RetrieveArrayByLength(Length));
        }

        public void Randomize() {
            for (int i = 0; i < a.Length; i++) {
                a[i] = Random.Range(2, 100);
            }
            strategy.Reset();
        }

        public int[] RetrieveArrayByLength(int length) {
            Length = length;
            return a.Take(length).ToArray();
        }

        public void SetStrategy(SortingStrategy strategy) {
            strategy?.Reset();
            this.strategy = strategy;
            strategy?.SetArray(a, Length);
        }

        public void SetLength(int length) {
            Length = length;
            strategy?.Reset();
            strategy?.SetArray(a, Length);
        }
    }
}
