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
                Text = "��e�]�Ƥ��AR�A���OAR�ݭn�w�˨�L�n��I",
            });
            Debug.Log("��e�]�Ƥ��AR�A���OAR�ݭn�w�˨�L�n��");
            yield return ARSession.Install();
        }
        if (ARSession.state == ARSessionState.Ready)
        {
            InfoTransceiver<UpdateDebugTextMsg>.Broadcast(new UpdateDebugTextMsg()
            {
                Text = "AR�w�ǳƴN���I",
            });
            Debug.Log("AR�w�ǳƴN��!");
        }
        else
        {
            switch (ARSession.state)
            {
                case ARSessionState.Unsupported:

                    InfoTransceiver<UpdateDebugTextMsg>.Broadcast(new UpdateDebugTextMsg()
					{
                        Text = "��e�]�Ƥ����AR�\��I",
                    });
                    Debug.LogWarning(GetType() + "/CheckSupport()/��e�]�Ƥ����AR�\��I");

                    break;
                case ARSessionState.NeedsInstall:

                    InfoTransceiver<UpdateDebugTextMsg>.Broadcast(new UpdateDebugTextMsg()
                    {
                        Text = "��e�]�Ƥ��AR�A���OAR�ݭn�w�˨�L�n��I",
                    });
                    Debug.Log("��e�]�Ƥ��AR�A���OAR�ݭn�w�˨�L�n��I");
                    break;

            }
        }
    }

}