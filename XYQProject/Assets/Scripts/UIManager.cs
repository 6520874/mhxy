using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： Trigger 
//功能说明：
//***************************************** 
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public GameObject playerOrderPanelGo;
    public GameObject bagPanelGo;
    public Slider hpSlider;
    public Slider mpSlider;
    public GameObject skillPanelGo;
    public GameObject fightCommandPanelGo;
    public Button[] skillBtn;
    public bool usingSkill;
    private int currentSkillID;
    public RawImage ri;
    private float blurValue;
    void Awake()
    {
        Instance = this;      
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)&&usingSkill)
        {
            usingSkill = false;
            ShowOrHidePlayerOrderPanel(true);
        }
        if (ri.gameObject.activeSelf)
        {
            blurValue += Time.deltaTime;
            ri.material.SetFloat("_Blur", blurValue);
            ri.color -= new Color(0,0,0,Time.deltaTime);
            if (ri.color.a<=0)
            {
                ri.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 显示或隐藏战斗指令
    /// </summary>
    /// <param name="state"></param>
    public void ShowOrHidePlayerOrderPanel(bool state)
    {
        playerOrderPanelGo.SetActive(state);
    }
    /// <summary>
    /// 驱动游戏逻辑
    /// </summary>
    private void PerformActLogic()
    {
        GameController.Instance.isPerformingLogic = false;
        GameController.Instance.enterCurrentRound = true;
        ShowOrHidePlayerOrderPanel(false);
    }
    /// <summary>
    /// 攻击指令按钮事件
    /// </summary>
    public void ClickAttackBtn()
    {
        CloseAllPanel();
        PerformActLogic();
        GameController.Instance.SetCharacterActCode(ActCode.ATTACK, GameController.Instance.playerTargetAI.attackPosTrans.position);
    }
    /// <summary>
    /// 防御指令按钮事件
    /// </summary>
    public void ClickDefendBtn()
    {
        CloseAllPanel();
        PerformActLogic();
        GameController.Instance.SetCharacterActCode(ActCode.DEFEND);
    }
    /// <summary>
    /// 技能指令按钮事件
    /// </summary>
    public void ClickUseSkillBtn()
    {
        bool open = !skillPanelGo.activeSelf;
        ClickUseSkillBtn(open);
        for (int i = 0; i < GameController.Instance.skillInfoList.Count-1; i++)
        {
            skillBtn[i].interactable = PlayerManager.Instance.currentMP 
                >= GameController.Instance.skillInfoList[i].decreateMPValue;         
        }
    }
    /// <summary>
    /// 点击某个技能
    /// </summary>
    /// <param name="skillID"></param>
    public void ClickSkill(int skillID)
    {
        usingSkill = true;
        currentSkillID = skillID;
        CloseAllPanel();
    }
    /// <summary>
    /// 选择完作用对象后使用技能，执行使用技能逻辑
    /// </summary>
    public void UseSKill()
    {
        usingSkill = false;
        CloseAllPanel();
        PerformActLogic();
        GameController.Instance.SetCharacterActCode(ActCode.SKILL, GameController.Instance.skillInfoList[currentSkillID]);
    }

    /// <summary>
    /// 使用技能指令按钮事件
    /// </summary>
    public void ClickUseSkillBtn(bool open)
    {
        bagPanelGo.SetActive(false);
        skillPanelGo.SetActive(open);
    }

    /// <summary>
    /// 使用物品指令按钮事件
    /// </summary>
    public void ClickUseItemBtn(bool open)
    {
        skillPanelGo.SetActive(false);
        bagPanelGo.SetActive(open);
    }
    /// <summary>
    /// 使用物品
    /// </summary>
    public void ClickItem(int itemID)
    {
        CloseAllPanel();
        PerformActLogic();
        GameController.Instance.SetCharacterActCode(ActCode.USEITEM, itemID);
    }
    /// <summary>
    /// 显示头像旁边的血量值
    /// </summary>
    /// <param name="value"></param>
    public void SetHPSliderValue(float value)
    {
        hpSlider.value = value;
    }
    /// <summary>
    /// 显示头像旁边的蓝量值
    /// </summary>
    /// <param name="value"></param>
    public void SetMPSliderValue(float value)
    {
        mpSlider.value = value;
    }
    /// <summary>
    /// 显示或隐藏战斗指令面板
    /// </summary>
    public void ShowOrHidefightCommandPanelGo(bool show)
    {
        fightCommandPanelGo.SetActive(show);
    }

    public void CloseAllPanel()
    {
        ShowOrHidefightCommandPanelGo(false);
        ClickUseItemBtn(false);
        ClickUseSkillBtn(false);
    }
    /// <summary>
    /// 相机截图
    /// </summary>
    /// <param name="camera">截屏相机</param>
    /// <param name="rect">截屏区域</param>
    /// <returns></returns>
    public Texture2D CaptureCamera(Camera camera,Rect rect)
    {
        RenderTexture rt = new RenderTexture((int)rect.width,(int)rect.height,0);
        camera.targetTexture = rt;
        camera.Render();
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width,(int)rect.height);
        screenShot.ReadPixels(rect,0,0);
        screenShot.Apply();
        camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        return screenShot;
    }
    /// <summary>
    /// 设置截屏UI所需材质的值
    /// </summary>
    /// <param name="texture"></param>
    public void SetMaterialValue(Texture texture)
    {
        ri.material.SetTexture("_MainTex",texture);
        ri.color = new Color(ri.color.r, ri.color.g, ri.color.b, 1);
        ri.gameObject.SetActive(true);
        blurValue = 0;
    }
}
