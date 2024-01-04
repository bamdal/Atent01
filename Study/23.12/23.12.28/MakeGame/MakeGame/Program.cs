namespace MakeGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("플레이 할 캐릭터의 이름을 입력하세요 : ");
            string? playerName = Console.ReadLine();


/*            player.Skill();

            Player player1 = new Player();
            player1.Skill();

            enemy.Skill();

            Enemy enemy1 = new Enemy();
            enemy1.Skill();
*/

            Random random = new Random();
            
            Player player = new Player(playerName);
            Enemy enemy = new Enemy("적");
            player.AttackPower = 1+ random.Next(50); 
            enemy.AttackPower = 1+ random.Next(50);




            while (player.IsAlive && enemy.IsAlive)
            {

/*                player.AttackPower = 1 + random.Next(50);
                enemy.AttackPower = 1 + random.Next(50);*/
/*                player.Skill();
                enemy.Skill();*/
                if(player.IsAlive)
                { player.Attack(enemy); }
                if(enemy.IsAlive)
                { enemy.Attack(player); }
                
               

            }
     
            // 실습
            // 적과 나 중에 한명이 죽을때까지 한번씩 공격하기
            // 죽을 때 누가 죽었는지 출력이 되어야 한다.
            // 한명이 죽으면 프로그램 종료

        }


    }
}
