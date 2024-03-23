using System;
using System.Threading.Tasks;
using UnityEngine;
namespace Project.Scripts.Algorithms {
    public abstract class SortingStrategy {
        public int[] A;
        public bool completed = false;
        protected int length;
        protected void Swap(int[] array, int i, int j) {
            (array[i], array[j]) = (array[j], array[i]);
        }
        public void SetArray(int[] a, int length) {
            A = a;
            this.length = length;
        }
        public abstract void Reset();
        public abstract int[] Step();
    }

    public class BubbleSort : SortingStrategy {
        int i = 0;
        int j = 0;
        public override void Reset() {
            i = 0;
            j = 0;
            completed = false;
        }
        public override int[] Step() {
            if (i < length) {
                if (j < length - i - 1) {
                    if (A[j] > A[j + 1]) {
                        Swap(A, j, j + 1);
                        return A;
                    }
                    j++;
                }
                else {
                    i++;
                    j = 0;
                }
            }
            else {
                completed = true;
                return A;
            }
            return Step();
        }
    }

    public class SelectionSort : SortingStrategy {
        int i = 0;
        int j = 0;
        int min = 0;
        bool jLoop = false;

        public override void Reset() {
            i = 0;
            j = 0;
            min = 0;
            completed = false;
            jLoop = false;
        }

        public override int[] Step() {
            if (i < length - 1) {
                if (!jLoop) {
                    min = i;
                    j = i + 1;
                    jLoop = true;
                }
                else {
                    if (j < length) {
                        if (A[j] < A[min]) {
                            min = j;
                        }
                        j++;
                        return Step();
                    }
                    else {
                        jLoop = false;
                        if (min != i) {
                            Swap(A, min, i);
                        }
                        i++;
                        return A;
                    }
                }
                return A;
            }
            else {
                completed = true;
                return A;
            }
        }
    }

    public class InsertionSort : SortingStrategy {
        int i = 1;
        int j = -1;
        int key = 0;
        bool jLoop = false;
        public override void Reset() {
            i = 1;
            j = -1;
            key = 0;
            jLoop = false;
            completed = false;
        }
        public override int[] Step() {
            if (i < length) {
                if (!jLoop) {
                    key = A[i];
                    j = i - 1;
                }
                if (j >= 0 && A[j] > key) {
                    jLoop = true;
                    Swap(A, j, j + 1);
                    var temp = (true, j, j + 1);
                    j--;
                    return A;
                }
                else {
                    A[j + 1] = key;
                    jLoop = false;
                    i++;
                    return A;
                }
            }
            else {
                completed = true;
                return A;
            }
        }
    }

    public class MergeSort : SortingStrategy {
        int[] tempArray;

        public override void Reset() {
            completed = false;
            tempArray = null;
        }

        public override int[] Step() {
            if (tempArray == null) {
                tempArray = new int[A.Length];
                Array.Copy(A, tempArray, A.Length);
                _ = StartMergeSort(A);
                return A;
            }
            return A;
        }
        
        async Task StartMergeSort(int[] array) {
            await MergeSortRecursive(array, 0, array.Length - 1);
            completed = true;
        }

        async Task Merge(int[] array, int left, int middle, int right) {
            int leftLength = middle - left + 1;
            int rightLength = right - middle;
            int[] leftArray = new int[leftLength];
            int[] rightArray = new int[rightLength];
            Array.Copy(array, left, leftArray, 0, leftLength);
            Array.Copy(array, middle + 1, rightArray, 0, rightLength);

            int i = 0, j = 0, k = left;
            while(i < leftLength && j < rightLength) {
                if (leftArray[i] <= rightArray[j]) {
                    array[k] = leftArray[i];
                    i++;
                }
                else {
                    array[k] = rightArray[j];
                    j++;
                }
                k++;
            }

            while(i < leftLength) {
                array[k] = leftArray[i];
                i++;
                k++;
            }

            while(j < rightLength) {
                array[k] = rightArray[j];
                j++;
                k++;
            }
        }

        async Task MergeSortRecursive(int[] array, int left, int right) {
            if (left < right) {
                int middle = left + (right - left) / 2;
                await MergeSortRecursive(array, left, middle);
                await MergeSortRecursive(array, middle + 1, right);
                await Awaitable.NextFrameAsync();
                await Merge(array, left, middle, right);
            }
        }
    }
}
