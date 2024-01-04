using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeGame
{
    internal class Enemy : Character 
    {

        public Enemy(string _name) : base(_name) 
        {
        
        }
        protected override float OnSkill()
        {
            //base.OnSkill();
            Console.WriteLine($"Enemy 적이 {AttackPower * 3.0f}의 데미지 스킬 사용");
            return AttackPower * 3.0f;
        }
    }
}
