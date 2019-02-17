public class BullSort {
    public int[] sort(int[] testArray){
        int[] result=new int[testArray.length];

        int dataMin=0;
        for(int i=0;i<testArray.length;i++){
            for(int j=i;j<testArray.length;j++){
                if(j==i){
                    dataMin=testArray[j];
                    continue;
                }
                if(dataMin<testArray[j]){
                    continue;
                }

                int temp=dataMin;
                dataMin=testArray[j];
                testArray[j]=temp;
            }
            result[i]=dataMin;
        }

        return  result;
    }

   public static void main(String[] args){
        BullSort BS=new BullSort();
        int[] testArray={10,2,3,65,9,-9,-2,8,-4,-9};
        for(int t:BS.sort(testArray)) {
            System.out.println(t);
        }
   }
}

