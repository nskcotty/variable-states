public class Main { 
   public static void method(boolean... conditions) { 
      int x = 1;
      if (conditions[0])
      {
        x=2;
        x=3;
      }
      if (conditions[1])
      {
          x = 4;
          if (conditions[2])
          {
              x = 5;
          }
      }
}
}