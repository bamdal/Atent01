using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeGame
{
    internal class Player : Character // Player 클래스가 Chracter 상속받은 C#은 한개의 부모만 상속받을수 있음
    {
        public Player(string _name) : base(_name) 
        {
        
        }


        public  void Attack()
        {
            Console.WriteLine($"Player 플레이어 에게 공격");
        }
        protected override float OnSkill()
        {
            //base.OnSkill();   // 부모 클래스에 있는 Skill()을 불러온다.
            Console.WriteLine($"Player 플레이어가 {AttackPower * 5.0f}의 데미지 스킬 사용");
            return AttackPower * 5.0f;
        }

    }
}
