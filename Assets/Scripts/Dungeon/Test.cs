using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode2
{
    public int data;
    public TreeNode2[] children;
    public int numChildren;
    public int maxChildren = 4;

    public TreeNode2(int value)
    {
        data = value;
        children = new TreeNode2[maxChildren];
        numChildren = 0;
    }

    // 자식 노드 추가 메서드
    public void AddChild(TreeNode2 child)
    {
        if (numChildren < maxChildren)
        {
            children[numChildren] = child;
            numChildren++;
        }
        else
        {
            Debug.LogWarning("Cannot add more children. Maximum children reached.");
        }
    }
}

public class Test : MonoBehaviour
{
    TreeNode2 root;
    public int maxDepth = 3; // 최대 깊이 설정

    void Start()
    {
        // 랜덤한 트리 생성
        root = GenerateRandomTree(0);
        
        // 트리 출력 (전위 순회)
        Debug.Log("Random Tree Traversal (Preorder): ");
        PreorderTraversal(root);
    }

    // 랜덤한 트리 생성 함수
    TreeNode2 GenerateRandomTree(int depth)
    {
        if (depth >= maxDepth)
            return null;

        TreeNode2 node = new TreeNode2(Random.Range(1, 100)); // 랜덤한 노드 생성

        int numChildren = Random.Range(0, 4); // 랜덤한 자식 수 선택 (0에서 3까지)
        for (int i = 0; i < numChildren; i++)
        {
            TreeNode2 childNode = GenerateRandomTree(depth + 1); // 재귀적으로 자식 노드 생성
            node.AddChild(childNode); // 생성된 자식 노드를 현재 노드에 추가
        }

        return node;
    }

    // 전위 순회 함수
    void PreorderTraversal(TreeNode2 node)
    {
        if (node != null)
        {
            Debug.Log(node.data); // 현재 노드 출력
            for (int i = 0; i < node.numChildren; i++)
            {
                PreorderTraversal(node.children[i]); // 자식 노드 순회
            }
        }
    }
}