#include <stdio.h>
#include <vector>
#include <iostream>
using namespace std;


int partition(vector<int>& arr, int low, int high)
{
    
    int pivot = arr[high];

    int i = low - 1;

    for (int j = low; j < high; j++)
    {
        if (arr[j] < pivot)
        {
            i++;
            swap(arr[i], arr[j]);
        }
    }

    swap(arr[i + 1], arr[high]);

    return i + 1;
}

void quickSort(vector<int>& arr, int low, int high)
{
    if (low < high)
    {
        int pi = partition(arr, low, high);

        quickSort(arr, low, pi - 1);
        quickSort(arr, pi, high);
    }
}



int main()
{
    vector<int> arr = { 12, 11, 13, 5, 6, 7, 10};

    cout << "정렬 전 배열 : ";
    for (int num : arr)
    {
        cout << num << " ";
    }

    cout << endl;

    quickSort(arr, 0, arr.size() - 1);

    cout << "정렬 후 배열 : ";
    for (int num : arr)
    {
        cout << num << " ";
    }

    cout << endl;
}