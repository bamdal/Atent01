using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour
{

    /// <summary>
    /// 경고 색(노랑)
    /// </summary>
    public Color errorColor = Color.yellow;

    /// <summary>
    /// 에러 색(빨강)
    /// </summary>
    public Color warningColor = Color.red;



    /// <summary>
    /// 로그창에 출력될 최대 줄 수 (안보이는것 포함)
    /// </summary>
    const int MaxLineCount = 20;
    
    /// <summary>
    /// 문자열 합치기 위한 스트링 빌더
    /// </summary>
    StringBuilder sb;

    /// <summary>
    /// 로그창에 출력될 모든 문자열 큐
    /// </summary>
    Queue<string> logLines = new Queue<string>(MaxLineCount+1);


    /// 컴포넌트들
    TextMeshProUGUI log;
    TMP_InputField inputField;

    private void Awake()
    {
        Transform child = transform.GetChild(3);
        inputField = child.GetComponent<TMP_InputField>();
        // inputField.onEndEdit;   // 입력이 끝났을 때 (엔터치거나 포커스를 잃었을 때 실행
        // inputField.onSubmit;    // 입력이 완료되었을 때 실행 (엔테를 쳤을때만 실행)
        inputField.onSubmit.AddListener((text) => 
        {
            
            Log(text);
            inputField.text = string.Empty;     // 입력 완료되면 비우기
            inputField.ActivateInputField();    // 포커스 다시 활성화
            //inputField.Select(); // 포커스 있을때는 비활성화 없을때는 활성화
        });

        child = transform.GetChild(0);
        child = child.GetChild(0);
        child = child.GetChild(0);

        log = child.GetComponent<TextMeshProUGUI>();
        sb = new StringBuilder(MaxLineCount + 1);
    }

    private void Start()
    {
        log.text = string.Empty;
    }

    /// <summary>
    /// 로거에 문자열을 추가하는 함수
    /// </summary>
    /// <param name="message">추가할 메시지</param>
    public void Log(string message)
    {


        // [] 사이에 글자는 빨간색(255,0,0)
        // {} 사이에 글자는 노란색(255,255,0)
        message = HighlightSubString(message, '[', ']', warningColor);
        message = HighlightSubString(message, '{', '}', errorColor);


        if (message != string.Empty)    // 빈내용은 추가 안함
        {
            logLines.Enqueue(message);          // 큐에 문자열 추가
            if (logLines.Count > MaxLineCount)  // 최대 줄수를 넘겼다면
            {
                logLines.Dequeue();             // 큐에서 하나 제거
            }
            sb.Clear();

            foreach (string line in logLines)   // 스틸링빌더에 큐에 문자열 추가
            {
                sb.AppendLine(line);            // 한줄씩 넘겨서 라인추가
            }
            //string logText = sb.ToString();
            //if(logText.Contains("{") && logText.Contains("}"))
            //{
            //    string colorText = ColorUtility.ToHtmlStringRGBA(errorColor);
            //    int index = logText.IndexOf("{");
            //    while (index != -1)
            //    {
            //        logText = logText.Insert(index, $"<#{colorText}>");
            //        index = logText.IndexOf("}", index + 1);
            //        if (index == -1)
            //        {
            //            logText = logText.Insert(logText.Length-1,"</color>");
            //            break;
            //        }
            //        logText = logText.Insert(index + 1, "</color>");
            //        index = logText.IndexOf("{",index+1);
            //    }
               
                

                
            //}

            //if (logText.Contains("[") && logText.Contains("]"))
            //{
            //    string colorText = ColorUtility.ToHtmlStringRGBA(warningColor);
            //    int index = logText.IndexOf("[");
            //    while (index != -1)
            //    {
            //        logText = logText.Insert(index, $"<#{colorText}>");
            //        index = logText.IndexOf("]", index + 1);
            //        if (index == -1)
            //        {
            //            logText = logText.Insert(logText.Length - 1, "</color>");
            //            break;
            //        }
            //        logText = logText.Insert(index + 1, "</color>");
            //        index = logText.IndexOf("[", index + 1);
            //    }

            //}

            //log.text = logText; 

            log.text = sb.ToString();   // 문자열 빌더에내용을 출력
        }

    }

    /// <summary>
    /// 인풋 필드에 포커스를 주는 함수
    /// </summary>
    public void InputFieldFocusOn()
    {
        inputField.ActivateInputField();
    }


    /// <summary>
    /// 지정된 괄호 사이에있는 글자 강조 함수
    /// </summary>
    /// <param name="source">원문</param>
    /// <param name="open">여는 괄호</param>
    /// <param name="close">닫는 괄호</param>
    /// <param name="color">강조할 색</param>
    /// <returns></returns>
    string HighlightSubString(string source, char open, char close, Color color)
    {
        string result = source;
        if (IsPair(source, open, close))    // source문자열 안에 괄호가 쌍으로 유효하게 존재하는지 판별(완전히 맞을때만 강조)
        {
            string[] split = source.Split(open,close);                  // 괄호를 기준으로 문자열 가르기
            string colorText = ColorUtility.ToHtmlStringRGBA(color);    // 출력용 컬러색 16진수로 변환

            result = string.Empty;                  // IsPair가 실패일 때는 result에 soure가 있는게 맞기 때문에 여기서 초기화
            for(int i = 0;i < split.Length; i++)    // 나누어 진것들을 하나씩 처리
            {
                result += split[i];                 // 나누어진 문자열을 result에 추가
                if (i != split.Length - 1)          // 마지막 문자열을 제외하고
                {
                    if (i % 2 == 0)                 // i가 짝수이면 이 이후에 괄호가 열려있고
                    {
                        result += $"<#{colorText}>{open}";  // 괄호가 열린부분부터 색 변화
                    }
                    else                            // i가 홀수이면 괄호를 닫는다
                    {
                        result += $"{close}</color>";       // 색상변경 정지
                    }
                }
            }
        }
        return  result;
    }

    /// <summary>
    /// source에 지정딘 괄호가 조건에 맞아 유효한지 검사하는 함수
    /// </summary>
    /// <param name="source">원문</param>
    /// <param name="open">여는 괄호</param>
    /// <param name="close">닫는 괄호</param>
    /// <returns>조건을 만족하면 true, 아니면 false</returns>
    bool IsPair(string source, char open, char close)
    {
        // 조건 : 괄호가 열리면 반드시 닫혀야 하고, 연속해서 열거나 닫는것도 안된다
        bool result = true;

        int count = 0;
        for(int i=0;i<source.Length;i++)    // source의 모든 글자를 확인
        {
            if (source[i] == open || source[i] == close)    // 여는 괄호이거나 닫는 괄호일 때
            {
                count++;        // 괄호 개수 증가
                if (count % 2 == 1) 
                {
                    if (source[i] != open)  // 홀수개 일때 열린괄호가 오지 않으면 틀림
                    {
                        result = false;
                        break;
                    }
                }
                else
                {
                    if (source[i] != close) // 짝수개 일때 닫힌 괄호가 오지 않으면 틀림
                    {
                        result = false;
                        break;
                    }
                }
            }
        }

        if(count == 0 || count % 2 != 0 ) // count의 개수가 짝수여야 하고 0인경우는 괄호가 없으니 변경할 이유가 없음
        {
            result = false;
        }
        return result;
    }
}
