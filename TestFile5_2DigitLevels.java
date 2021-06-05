public class Main { 
   public static void method(boolean... conditions) { 
      int x = 1;
      if (conditions[0])
      {
        x=2;
        if (conditions[1])
        {
            if (conditions[2])
            {
                if (conditions[3])
                {
                    if (conditions[4])
                    {
                        if (conditions[5])
                        {
                            x=3;
                        }
                    }
                }
            }
        }
        x=4;
        {
            x = 9;
            x= 44;
        }
        if (conditions[6])
        {
            if (conditions[7])
            {
                if (conditions[8])
                {
                    x=5;
                    {
                        x = 7;
                    }
                 }
             }
         }
      if (conditions[9])
      {
        x = 6;
      }
}

}
}