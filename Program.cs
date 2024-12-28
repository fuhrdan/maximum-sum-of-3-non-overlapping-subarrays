/**
 * Note: The returned array must be malloced, assume caller calls free().
 */
 //****************************************************************************
 //** 689. Maximum Sum of 3 Non-Overlapping Subarrays               leetcode **
 //****************************************************************************
 
typedef struct {
    int* data;
    int size;
    int capacity;
} Vector;

void vectorInit(Vector* vec) {
    vec->size = 0;
    vec->capacity = 10;
    vec->data = (int*)malloc(vec->capacity * sizeof(int));
}

void vectorPushBack(Vector* vec, int value) {
    if (vec->size == vec->capacity) {
        vec->capacity *= 2;
        vec->data = (int*)realloc(vec->data, vec->capacity * sizeof(int));
    }
    vec->data[vec->size++] = value;
}

void vectorFree(Vector* vec) {
    free(vec->data);
}

int** mem;
int* prefixSum;

int findMaxSum(int* nums, int numsSize, int pos, int count, int k) {
    if (count == 3) return 0;
    if (pos > numsSize - k) return 0;
    if (mem[pos][count] != -1) return mem[pos][count];

    // Don't start subarray here
    int dontStart = findMaxSum(nums, numsSize, pos + 1, count, k);

    // Start subarray here
    int startHere = findMaxSum(nums, numsSize, pos + k, count + 1, k) + prefixSum[pos + k] - prefixSum[pos];

    mem[pos][count] = dontStart > startHere ? dontStart : startHere;
    return mem[pos][count];
}

void findMaxSumPath(int* nums, int numsSize, int pos, int count, int k, Vector* path) {
    if (count == 3 || pos > numsSize - k) return;

    int dontStart = findMaxSum(nums, numsSize, pos + 1, count, k);
    int startHere = findMaxSum(nums, numsSize, pos + k, count + 1, k) + prefixSum[pos + k] - prefixSum[pos];

    if (startHere >= dontStart) {
        vectorPushBack(path, pos);
        findMaxSumPath(nums, numsSize, pos + k, count + 1, k, path);
    } else {
        findMaxSumPath(nums, numsSize, pos + 1, count, k, path);
    }
}

int* maxSumOfThreeSubarrays(int* nums, int numsSize, int k, int* returnSize) {
    // Initialize memory
    mem = (int**)malloc(numsSize * sizeof(int*));
    for (int i = 0; i < numsSize; i++) {
        mem[i] = (int*)malloc(3 * sizeof(int));
        memset(mem[i], -1, 3 * sizeof(int));
    }

    // Calculate prefix sum
    prefixSum = (int*)malloc((numsSize + 1) * sizeof(int));
    prefixSum[0] = 0;
    for (int i = 0; i < numsSize; ++i) {
        prefixSum[i + 1] = prefixSum[i] + nums[i];
    }

    // Compute max sum
    findMaxSum(nums, numsSize, 0, 0, k);

    // Compute subarray indices
    Vector path;
    vectorInit(&path);
    findMaxSumPath(nums, numsSize, 0, 0, k, &path);

    // Prepare result
    *returnSize = path.size;
    int* result = (int*)malloc(path.size * sizeof(int));
    memcpy(result, path.data, path.size * sizeof(int));

    // Free resources
    vectorFree(&path);
    for (int i = 0; i < numsSize; i++) free(mem[i]);
    free(mem);
    free(prefixSum);

    return result;
}