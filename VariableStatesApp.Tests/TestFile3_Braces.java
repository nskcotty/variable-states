public class Main { 
   public static void method(boolean... conditions) 
   { 
      int x = 1;
      if (conditions[0])
      {
            x=2;
            if (conditions[1])
            {
                x=3;
            }
            x=4;
            if (conditions[2])
            {
                x=5;
                {
                    x = 145;
                    x = 200;
                }
            }
          if (conditions[3])
          {
              x = 6;
              {
                  x = 97;
                  {x = 56;}
              }
          }
      }
   }
}