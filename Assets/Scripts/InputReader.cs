using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class InputReader : MonoBehaviour
{
    const int axes_count = 16;
    const int inputs_count = 7;
   

public static float[] joy_axes = new float[axes_count];

    static Transform canvas_transform;
    static GameObject[] uiSlider_arr = new GameObject[axes_count];
    static GameObject[] uiToggle_arr = new GameObject[axes_count];
    static GameObject[] uiDropdown_arr = new GameObject[axes_count];

    public static float throttle;
    public static float yaw;
    public static float pitch;
    public static float roll;
    public static bool arm;
    public static bool fire;


    class InputMappingEntry
    {
        public string name;
        public int axis_num;
        public bool inverted = false;
    }
    static InputMappingEntry[] mappingArray = new InputMappingEntry[]
    {
        new InputMappingEntry(){ axis_num = 0, name  = "Trottle",  },
        new InputMappingEntry(){ axis_num = 1, name  = "Yaw",  },
        new InputMappingEntry(){ axis_num = 2, name  = "Pitch",  },
        new InputMappingEntry(){ axis_num = 3, name  = "Roll",  },
        new InputMappingEntry(){ axis_num = 7, name  = "Arm",  },
        new InputMappingEntry(){ axis_num = 8, name  = "Fire",  }
    };


    private void Start()
    {
        canvas_transform = transform.Find("Canvas");

        canvas_transform.Find("Button").GetComponent<Button>().onClick.AddListener(ApplyChanges);

        //лист воможных инпутов
        List<Dropdown.OptionData> m_Messages = new List<Dropdown.OptionData>();
        m_Messages.Add(new Dropdown.OptionData() { text = "None" });
        foreach (InputMappingEntry e in mappingArray) //Добавляем все возможные инпуты
            m_Messages.Add(new Dropdown.OptionData() { text = e.name });

        //https://stackoverflow.com/questions/41194515/unity-create-ui-control-from-script
        DefaultControls.Resources uiResources = new DefaultControls.Resources();
        uiResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd");
        for (int i = 0; i < axes_count; i++)
        {
            uiSlider_arr[i] = DefaultControls.CreateSlider(uiResources);
            uiSlider_arr[i].transform.SetParent(canvas_transform, false);            
            uiSlider_arr[i].GetComponent<Slider>().minValue = -1;
            uiSlider_arr[i].GetComponent<Slider>().maxValue = 1;
            uiSlider_arr[i].transform.position = new Vector3(100, 1200 - i * 70, 0);

            uiToggle_arr[i] = DefaultControls.CreateToggle(uiResources);
            
            uiToggle_arr[i].transform.SetParent(canvas_transform, false);
            uiToggle_arr[i].GetComponent<Toggle>().transform.Find("Label").GetComponent<Text>().text = "Invert"; 
            uiToggle_arr[i].GetComponent<Toggle>().isOn = false; //TODO read
            uiToggle_arr[i].transform.position = new Vector3(400, 1200 - i * 70, 0);

            uiDropdown_arr[i] = DefaultControls.CreateDropdown(uiResources);
            uiDropdown_arr[i].transform.SetParent(canvas_transform, false);
            uiDropdown_arr[i].GetComponent <Dropdown>().ClearOptions();                      
            foreach (Dropdown.OptionData message in m_Messages) 
                uiDropdown_arr[i].GetComponent<Dropdown>().options.Add(message);
            var me = mappingArray.FirstOrDefault((t) => t.axis_num == i); //ищем на что забита эта ось
            if (EqualityComparer<InputMappingEntry>.Default.Equals(me, default(InputMappingEntry))) //если не нашелся элемент
                uiDropdown_arr[i].GetComponent<Dropdown>().value = 0;
            else
                uiDropdown_arr[i].GetComponent<Dropdown>().value = me.axis_num + 1;
            uiDropdown_arr[i].transform.position = new Vector3(500, 1200 - i * 70, 0);
        }
        //print(uiSlider.transform.position);
    }

    void FixedUpdate()
    {
        UpdateAxes();
    }
    private void Update()
    {
        for (int i = 0; i < axes_count; i++)
        {
            uiSlider_arr[i].GetComponent<Slider>().value = joy_axes[i];            
        }
    }

    private static void UpdateAxes()
    {
        for (int i = 0; i < axes_count; i++)
        {
            joy_axes[i] = Input.GetAxis($"ax{i + 1}");
        }

        throttle = joy_axes[mappingArray[0].axis_num];
        yaw = joy_axes[mappingArray[1].axis_num];
        pitch = joy_axes[mappingArray[2].axis_num];
        roll = joy_axes[mappingArray[3].axis_num];
        arm = joy_axes[mappingArray[4].axis_num] > 0.1; //TODO константу
        fire = joy_axes[mappingArray[5].axis_num] > 0.1;
        if (mappingArray[0].inverted) throttle = -throttle;
        if (mappingArray[1].inverted) yaw = -yaw;
        if (mappingArray[2].inverted) pitch = -pitch;
        if (mappingArray[3].inverted) roll = -roll;
        if (mappingArray[4].inverted) arm = !arm;
        if (mappingArray[5].inverted) fire = !fire;
    }

    void ApplyChanges()
    {
        for (int i = 0; i < axes_count; i++)
        {
            int input_num = uiDropdown_arr[i].GetComponent<Dropdown>().value;
            bool inverted = uiToggle_arr[i].GetComponent<Toggle>().isOn;
            if (input_num > 0)
            {
                mappingArray[input_num - 1].axis_num = i; //+1 т.к. первый None
                mappingArray[input_num - 1].inverted = inverted;
            }
        }
    }
}