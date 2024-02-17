using UnityEngine;
namespace Dafhne.Stage
{
    public static class StageReader
    {
       public static StageInfo LoadStage(int nStage)
       {
            Debug.Log($"LoadStage : Stage/{GetStageFileName(nStage)}");
            
            //1. 리소스에서 파일을 읽어온다 .
            TextAsset textAsset = Resources.Load<TextAsset>($"Stage/{GetStageFileName(nStage)}");
            if(textAsset != null)
            {
                //2. Json 문자열을 객체(StageInfo)로 변환한다. 
                StageInfo stageInfo = JsonUtility.FromJson<StageInfo>(textAsset.text);

                //3. 변환된 객체가 유효한지 체크해봄(디버깅 전용)

                Debug.Assert(stageInfo.isValidated());
                return stageInfo;
            }

            return null;
       }

       static string GetStageFileName(int nStage)
       {
            return string.Format("Stage_{0:D4}", nStage);
       }
    }
}

