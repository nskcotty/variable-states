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
                x=1;
            }
            x = 1;
            x = 11;
            x  =1;
            if (conditions[3])
            {
                x = 8;
            }
      }
   }
}