using System;
using System.Collections.Generic;
using System.Text;


namespace MakeGame
{
    class Character
    {
        string name;
        float maxHP;
        float hp;
        float maxMP;
        float mp;
        float attackPower;
        float defencePower;
        float level;
        float exp;
        const float maxExp = 100;

        public string Name { get => name; set => name = value; }
        public float MaxHP { get => maxHP; set => maxHP = value; }
        public float Hp 
        { 
            get => hp;
            private set 
            {
                if (hp != value) // HP의 변경이 일어났을때 알 수 있는 코드
                {
                   
                }
               
                hp = value;
                hp = Math.Clamp(value, 0, MaxHP); // value값이 0 ~ MaxHP 까지
                Console.WriteLine($"[{name}]의 현재 HP는 {hp}다");
                if (hp <= 0)
                {
                    Die();
                }
            }  
        
        }
        public float MxMP { get => maxMP; set => maxMP = value; }
        public float Mp { get => mp; set => mp = value; }
        public float AttackPower { get => attackPower; set => attackPower = value; }
        public float DefencePower { get => defencePower; set => defencePower = value; }
        public float Level { get => level; set => level = value; }
        public float Exp { get => exp; set => exp = value; }

        public bool IsAlive => hp > 0; //플레이어가 살아있는지 죽었는지 확인하는 프로퍼티
        protected const float skillCost = 10.0f;
        private bool CanSkillUse => mp>skillCost;

        //string Name => name; Name 프로퍼티를 읽기 전용으로 만들고 읽을려면 name 을 부른다.
        Random random;
        public Character() 
        {
            hp = 100;
            maxHP = 100;
            mp = 50;
            maxMP = 50;
            level = 1;
            exp = 0.0f;
            attackPower = 10.0f;
            defencePower = 5.0f;
            name = "무명";

            random = new Random();
        }

        public Character(string _name)
        {
            hp = 100;
            maxHP = 100;
            mp = 50;
            maxMP = 50;
            level = 1;
            exp = 0.0f;
            attackPower = 10.0f;
            defencePower = 5.0f;
            this.name = _name;

            random = new Random();
        }



        public float DamageResult(float Damage, float DEF, float AttackLV, float HitLV)
        {
            float result;
            result = Damage * ((AttackLV + 100) / ((AttackLV + 100) + (HitLV + 100) * ((10 + DEF) / 10)));
        
            return result;
        }


        public virtual void Attack(Character target)
        {
            // Console.WriteLine($"[{name}]이 {DamageResult(target.attackPower, defencePower, target.level, level)}의 데미지로 공격");
            if (CanSkillUse)
            {
                
                if (random.NextSingle() < 0.3f)
                {
                    Skill(target);
                    
                }
                else
                {
                    Console.WriteLine($"[{name}가 {attackPower}의 데이지로 공격한다.]");
                    target.Defence(attackPower);
                }
            }
            else
            {
                Console.WriteLine($"[{name}가 {attackPower}의 데이지로 공격한다.]");
                target.Defence(attackPower);
            }
            
           
        }

        public void Defence(float _attackPower)
        {
            
            Hp -= _attackPower;
           
          
        }
        public void Skill(Character target) // virtual 함수는 상속받은 클래스에서 덮어쓸수있다 (override 가능)
        {
            if(mp > skillCost)
            {
                mp -= skillCost;
                target.Defence( OnSkill());
               
            }

        }

       protected virtual float OnSkill()
        { 
            Console.WriteLine("Charater 스킬 사용");
            return 10.0f;
        }

        void LevelUp()
        {

        }

        void Die()
        {
            Console.WriteLine($"{name} 이 사망했습니다");
        
        }

    }
}
