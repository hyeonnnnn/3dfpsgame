using UnityEngine;

public class TryCatchFinallyTest : MonoBehaviour
{
    public int Age;

    private void Start()
    {
        if (Age < 0)
        {
            Debug.LogError("Age cannot be negative.");
            // throw new Exception();
        }

        int[] numbers = new int[] { 1, 2, 3 };

        try
        {
            numbers[7] = 1;
        }
        catch
        {
            int index = numbers.Length - 1;
            numbers[index] = 1;
        }
        finally
        {
            Debug.Log("Execution of try-catch block is complete.");
        }
        // Try-catch 구문은 되도록이면 안 쓰는 게 좋다.
        // - 성능 저하 (이 이유가 크다.)
        // - 잘못된 알고리즘

        // 써야 하는 경우: 내가 제어할 수 없을 때
        // 1. 네트워크 접근
        // - 로그인, 로그아웃 / 서버 / DB 아이템 저장, 불러오기
        // 2. 파일 접근
        // - 용량 충분? 파일명 괜춘? 권한 있음?
        // 3. DB 접근
    }
}
