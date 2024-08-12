using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CheckARSupport : InitializeMonoBehaviour
{
    public override void Initialize()
    {
        StartCoroutine(CheckSupport());
    }

    private IEnumerator CheckSupport()
    {
        yield return ARSession.CheckAvailability();

        if (ARSession.state == ARSessionState.NeedsInstall)
        {
            InfoTransceiver<UpdateDebugTextMsg>.Broadcast(new UpdateDebugTextMsg()
            {
                Text = "當前設備支持AR，但是AR需要安裝其他軟件！",
            });
            Debug.Log("當前設備支持AR，但是AR需要安裝其他軟件");
            yield return ARSession.Install();
        }
        if (ARSession.state == ARSessionState.Ready)
        {
            InfoTransceiver<UpdateDebugTextMsg>.Broadcast(new UpdateDebugTextMsg()
            {
                Text = "AR已準備就緒！",
            });
            Debug.Log("AR已準備就緒!");
        }
        else
        {
            switch (ARSession.state)
            {
                case ARSessionState.Unsupported:

                    InfoTransceiver<UpdateDebugTextMsg>.Broadcast(new UpdateDebugTextMsg()
					{
                        Text = "當前設備不支持AR功能！",
                    });
                    Debug.LogWarning(GetType() + "/CheckSupport()/當前設備不支持AR功能！");

                    break;
                case ARSessionState.NeedsInstall:

                    InfoTransceiver<UpdateDebugTextMsg>.Broadcast(new UpdateDebugTextMsg()
                    {
                        Text = "當前設備支持AR，但是AR需要安裝其他軟件！",
                    });
                    Debug.Log("當前設備支持AR，但是AR需要安裝其他軟件！");
                    break;

            }
        }
    }

}