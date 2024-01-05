using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar : MonoBehaviour
{

    public class Node
    {
        public Node(bool _isWall, int _x, int _y) { isWall = _isWall; x = _x; y = _y; }

        public bool isWall;
        public Node ParentNode;


        public int x, y, G, H;

        public int F { get { return G + H;  } }

    }

    public Vector2Int monsterPos, playerPos, detectionbottomLeft, detectiontopRight;
    public List<Node> FinalNodeList;
    public GameObject player;
    Vector3 playerV3, enemyV3;


    int sizeX, sizeY;
    Node[,] NodeArray;
    Node StartNode, TargetNode, CurNode;
    List<Node> OpenList, ClosedList;

    public void PathFinding()
    {

        playerV3 = player.transform.position;
        enemyV3 = this.transform.position;
        playerPos = new Vector2Int((int)playerV3.x, (int)playerV3.y);
        monsterPos = new Vector2Int((int)enemyV3.x, (int)enemyV3.y);


        sizeX = detectiontopRight.x - detectionbottomLeft.x + 1;
        sizeY = detectiontopRight.y - detectionbottomLeft.y + 1;
        NodeArray = new Node[sizeX, sizeY];

        for(int i = 0; i < sizeX; i++)
        {
            for(int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                foreach(Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i+detectionbottomLeft.x,j+detectionbottomLeft.y), 0.4f))
                    if(col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true; // 벽 감지 하기

                NodeArray[i,j] = new Node(isWall, i+ detectionbottomLeft.x, j+detectionbottomLeft.y); // 감지할 공간을 배열로 설정
            }
        }

        StartNode = NodeArray[monsterPos.x - detectionbottomLeft.x, monsterPos.y - detectionbottomLeft.y];
        TargetNode = NodeArray[playerPos.x-detectionbottomLeft.x,playerPos.y-detectionbottomLeft.y];

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();

        while (OpenList.Count > 0)
        {
            CurNode = OpenList[0];
            for(int i = 0; i<OpenList.Count; i++) {
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H) { CurNode = OpenList[i]; }
            }

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);

            if(CurNode == TargetNode) 
            {
                Node TargetCurNode = TargetNode;
                while(TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();
            }

            OpenListAdd(CurNode.x+1, CurNode.y+1);
            OpenListAdd(CurNode.x-1, CurNode.y+1);
            OpenListAdd(CurNode.x-1, CurNode.y-1);
            OpenListAdd(CurNode.x + 1, CurNode.y + 1);
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x+1,CurNode.y);
            OpenListAdd(CurNode.x,CurNode.y-1);
            OpenListAdd(CurNode.x - 1, CurNode.y);

        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
        if(checkX >= detectionbottomLeft.x && checkX<detectiontopRight.x +1 // 감지범위 안
            && checkY>= detectionbottomLeft.y && checkY < detectiontopRight.y+1 // 감지범위 안
            && !NodeArray[checkX- detectionbottomLeft.x,checkY - detectionbottomLeft.y].isWall // 벽이 아님
            && !(NodeArray[CurNode.x-detectionbottomLeft.x, checkY - detectionbottomLeft.y].isWall // 벽사이로 못지나감
            && NodeArray[checkX-detectionbottomLeft.x,CurNode.y - detectionbottomLeft.y].isWall) // 코너돌때 장애물 없어야됨
            && !ClosedList.Contains(NodeArray[checkX-detectionbottomLeft.x,checkY-detectionbottomLeft.y])) // 닫힌리스트에 없음
        {
            Node NeighborNode = NodeArray[checkX-detectionbottomLeft.x,checkY-detectionbottomLeft.y];
            int MoveCost = CurNode.G + (CurNode.x - checkX ==0 || CurNode.y - checkY ==0 ? 10 : 14);

            if(MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }

        }
    }

    void OnDrawGizmos()
    {
        if (FinalNodeList.Count != 0) for (int i = 0; i < FinalNodeList.Count - 1; i++)
                Gizmos.DrawLine(new Vector2(FinalNodeList[i].x, FinalNodeList[i].y), new Vector2(FinalNodeList[i + 1].x, FinalNodeList[i + 1].y));
    }


    // Start is called before the first frame update
    private void Awake()
    {
        // public Vector2Int monsterPos, playerPos, detectionbottomLeft, detectiontopRight;
        
        playerV3 = player.transform.position;
        enemyV3 = this.transform.position;
        playerPos = new Vector2Int((int)playerV3.x,(int)playerV3.y);
        monsterPos = new Vector2Int((int)enemyV3.x,(int)enemyV3.y);
      
        InvokeRepeating(nameof(PathFinding), 0.0f, 1.0f);
        //CancelInvoke(nameof(PathFinding));


    }


    private void Update()
    { 


        
        
    }
}
