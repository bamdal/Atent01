
using static System.Formats.Asn1.AsnWriter;

namespace ConsoleApp1
{
    public enum Grade
    { 
        A,
        B, 
        C, 
        D, 
        F,X
    }
    public enum DayOfWeek
    { 
        monday,
        tuesday,
        wendseday,
        thurday,
        friday,
        saturday,
        sunday
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            //InputName();
            // AgeCheck(age);
            string str1 = "Hello";
            string str2 = "World";
            float grade = 0;


            /*            float.TryParse(Console.ReadLine(), out grade);
                        Grade gra = GradeCheck(grade);
                        Gradestart(gra);
                        Console.WriteLine($"str = {str1} {gra}");*/
            /*            int.TryParse(Console.ReadLine(), out int gugudan);
                        GuGuDan(gugudan);*/
            // 삼항 연산자 bool isHigh = (dice > 3) ? true : false; (조건) ? 참일때 값 : 거짓일때 값
            PlayGame();

        }

        private static void PlayGame()
        {
            int chooseGame = 3;
            do
            {
                Console.WriteLine("0(주사위게임),1(홀짝게임) 선택");
                int.TryParse(Console.ReadLine(), out chooseGame);

                if (!(chooseGame == 0 || chooseGame == 1))
                    Console.WriteLine("잘못입력");
            } while (!(chooseGame == 0 || chooseGame == 1));
            if (chooseGame == 0)
            {
                int playerWinCount = 0;
                Console.WriteLine("주사위 게임 HIGH,LOW");
                do
                {
                    Random r = new Random();
                    int dice = r.Next(5) + 1;
                    Console.WriteLine(dice);
                    string? playerDice = Console.ReadLine();
                    playerDice = playerDice.ToUpper();

                    bool isHigh = (dice > 3) ? true : false;

                    if (dice < 4)
                    {
                        Console.WriteLine("LOW");
                    }
                    else
                        Console.WriteLine("HIGH");

                    if ((playerDice == "HIGH" || playerDice == "1") && (isHigh == true))
                    {
                        Console.WriteLine("HIGH정답");
                        playerWinCount++;
                    }
                    else if ((playerDice == "LOW" || playerDice == "0") && (isHigh == false))
                    {
                        playerWinCount++;
                        Console.WriteLine("LOW정답");
                    }
                    else
                    {
                        Console.WriteLine($"{playerWinCount}번 성공");

                        break;
                    }

                } while (true);
            }
            else if (chooseGame == 1)
            {
                int playerWinCount = 0;
                Console.WriteLine("홀짝 게임 홀,짝");
                do
                {
                    Random r = new Random();
                    int dice = (r.Next(5) + 1) % 2;
                    Console.WriteLine(dice);
                    string? playerDice = Console.ReadLine();
                    playerDice = playerDice.ToUpper();

                    if (dice == 0)
                    {
                        Console.WriteLine("짝");
                    }
                    else if (dice == 1)
                        Console.WriteLine("홀");

                    if ((playerDice == "짝" || playerDice == "0") && dice == 0)
                    {
                        Console.WriteLine("짝 정답");
                        playerWinCount++;
                    }
                    else if ((playerDice == "홀" || playerDice == "1") && dice == 1)
                    {
                        playerWinCount++;
                        Console.WriteLine("홀 정답");
                    }
                    else
                    {
                        Console.WriteLine($"{playerWinCount}번 성공");

                        break;
                    }

                } while (true);
            }
            else
            {
                Console.WriteLine("잘못선택");
            }
        }

        static void GuGuDan(int dan)
        {
            if(dan == 0) 
            {
                Console.WriteLine("1 이상의 숫자만 입력");
                return;
            }
            Console.WriteLine($"구구단의 {dan}단까지는");
            for (int i = 1; i <= dan; i++) 
            {
                for (int j = 1; j < 10; j++) 
                {
                    Console.Write($"{j} X {i} = {i * j}\t");
                }
                Console.WriteLine();
                
            }
        }

        private static void Gradestart(Grade gra)
        {
            switch (gra)
            {
                case Grade.A:
                    Console.WriteLine($"{gra}학점입니다");
                    break;
                case Grade.B:
                    Console.WriteLine($"{gra}학점입니다");
                    break;
                case Grade.C:
                    Console.WriteLine($"{gra}학점입니다");
                    break;
                case Grade.D:
                    Console.WriteLine($"{gra}학점입니다");
                    break;
                case Grade.F:
                    Console.WriteLine($"{gra}학점입니다");
                    break;
                default:
                    Console.WriteLine($"빠꾸");
                    break;

            }
        }

        /// <summary>
        /// 리턴 타입 : void
        /// 이름 : InputName
        /// 파라메터 : 생략되었음, () 안에 있는 변수
        /// 함수바디 : {} 사이에 있는 코드
        /// </summary>
        static void InputName()
        {
            Console.Write("이름이 무엇인가요? :");
            string s = Console.ReadLine();
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(s);
            }
        }
        static void AgeCheck(int age) 
        {
            if (age < 8)
            {
                Console.WriteLine($"{age}살은 미취학 아동입니다");

            }
            else if (age < 14)
            {
                Console.WriteLine($"{age}살은 초등학생입니다");
            }
            else if (age < 17)
            {

                Console.WriteLine($"{age}살은 중학생입니다");
            }
            else if (age < 20)
            {

                Console.WriteLine($"{age}살은 고등학생입니다");
            }
            else
            {
                Console.WriteLine($"{age}살은 성인입니다");
            }
        }

        /// <summary>
        /// 정수를 받아서 A~F까지 성적을 출력하는 함수
        /// 90이상 A
        /// 80이상 B
        /// 70이상 C
        /// 60이상 D
        /// 60미만 F
        /// </summary>
        /// <param name="score"></param>
        static Grade GradeCheck(float score)
        {
            Grade grade = Grade.F;
            if (score < 0)
            {
                Console.WriteLine("1~100 사이 입력");
                grade = Grade.X;
            }
            else if (score < 60)
            { 
                Console.WriteLine($"{score}는 {grade}학점입니다");
            }
            else if (score < 70)
            { 
                grade = Grade.D;
                Console.WriteLine($"{score}는 {grade}학점입니다");

            }
            else if (score < 80)
            {
                grade = Grade.C;
                Console.WriteLine($"{score}는 {grade}학점입니다");
            }
            else if (score < 90)
            {
                grade = Grade.B;
                Console.WriteLine($"{score}는 {grade}학점입니다");
            }
            else if (score > 100 )
            {
                Console.WriteLine("1~100 사이 입력");
                grade = Grade.X;
            }
            else
            {
                grade = Grade.A;
                Console.WriteLine($"{score}는 {grade}학점입니다");
            }

            return grade;
        }

    }
}

