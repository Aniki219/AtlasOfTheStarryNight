// GENERATED AUTOMATICALLY FROM 'Assets/InputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMaster : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""ZXCArrows"",
            ""id"": ""b4fb1bdf-43b4-4b2b-83a0-7c11ecf2e2b8"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""7d36bc24-f343-4751-8aa4-430723928408"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""7e3032a4-d648-4ee7-9c13-e7669f09644e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""19823573-1db7-4a31-a207-10480e49260f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Broom"",
                    ""type"": ""Button"",
                    ""id"": ""49bee84f-4897-4d18-bfb5-3c74d58ff011"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""e6765054-693b-47ab-b154-34e7c877db56"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Down"",
                    ""type"": ""Button"",
                    ""id"": ""a557e63b-7d36-492e-b993-6197fed50141"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Up"",
                    ""type"": ""Button"",
                    ""id"": ""0502b492-0981-4945-a995-2a180477f461"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DisplayEquipment"",
                    ""type"": ""Button"",
                    ""id"": ""8053b2f1-8296-4dbe-8447-b8e94ba9896a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cheat"",
                    ""type"": ""Button"",
                    ""id"": ""a7af950a-f991-4fde-b288-45b063472fe4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Escape"",
                    ""type"": ""Button"",
                    ""id"": ""dd34645f-0bfd-439c-8ebd-4a2eda762d79"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Dpad"",
                    ""id"": ""69e8247e-4c4a-4f66-8f40-8528a74528bb"",
                    ""path"": ""2DVector(mode=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""0b64c561-9d0b-403e-b342-41d193b21cee"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""66dd9d39-841c-4848-9979-a99fb82c312d"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b6d60cdc-d853-4385-b3d1-29c2b9783dbb"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""73d0699e-cc84-4b17-b551-c3d1cf218f4a"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""PS4 Dpad"",
                    ""id"": ""b5ecbcd6-1d42-49cc-82c7-d9341f1b16f6"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""82d75b08-7f44-47ea-8ef9-2f2bd88470d6"",
                    ""path"": ""<DualShockGamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""d29657ea-6b70-4688-ac07-8cb41fa63288"",
                    ""path"": ""<DualShockGamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8384b5df-57fd-4250-b95d-e8bb25fb2e1e"",
                    ""path"": ""<DualShockGamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b8a0a27b-9296-467f-8b59-d27abaa551ae"",
                    ""path"": ""<DualShockGamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""caf07544-cba8-4393-9d1a-1250450f19ba"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6192a8cc-a26d-42c3-a912-7bcebe5c99b3"",
                    ""path"": ""<DualShockGamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""76568dea-0cb8-486d-8dce-6c5f249d26cb"",
                    ""path"": ""<DualShockGamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c44f4ce8-7b26-4a45-be09-8dac5de275b9"",
                    ""path"": ""<DualShockGamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""433e50a5-d893-478a-9c7e-9cae38ccf2f2"",
                    ""path"": ""<DualShockGamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f0d1d5c4-ffb3-4f93-aec7-c8a940d21e1a"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""09edfa90-936f-41f9-87de-a4de319bd9e5"",
                    ""path"": ""<DualShockGamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""395b4cee-7266-45e2-9234-37579db2370f"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""010fb29b-ac39-4c93-aa62-f7fcb93e080e"",
                    ""path"": ""<DualShockGamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aade17ed-7811-40a9-914c-b4d0e7a2b66c"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Broom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""061d7a7e-c21f-4b01-9203-98ec0c639389"",
                    ""path"": ""<DualShockGamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Broom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fd17d471-ce0a-4a2a-9319-ab894cce065d"",
                    ""path"": ""<DualShockGamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Broom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4af8d0a1-9980-40ee-b4b7-7c2bd993d64c"",
                    ""path"": ""<DualShockGamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Broom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""578a5239-5e3d-47fc-8181-4568513e4658"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""37ec1c2f-e14c-44bc-a105-4e60c08f68e9"",
                    ""path"": ""<DualShockGamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fae33a42-452b-4a30-a7e3-7d4b1dfa7376"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5d483097-111e-449f-8c7e-05f01e34e7c6"",
                    ""path"": ""<DualShockGamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a3223b19-7338-45fb-8e9d-0b00608f8078"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DisplayEquipment"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""238333c3-0f30-4605-b3ae-91ff07107ac3"",
                    ""path"": ""<DualShockGamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DisplayEquipment"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a3be7369-7f55-4778-8826-de9bfda18224"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a63bdcce-373b-4f40-a1cd-866844ae0d29"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""594ab810-d4ba-40f8-b6ef-e0dc4025f06b"",
                    ""path"": ""<Keyboard>/numpadMultiply"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cheat"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eb13e24d-39b8-47f1-9bd4-0f085985cef9"",
                    ""path"": ""<DualShockGamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Escape"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a6019c1f-d9e2-42a7-a06e-04aef67ed695"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Escape"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PS4"",
            ""id"": ""d03ea653-ba62-454e-9785-9557adfeb686"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""af5722d8-771f-4af5-9e2d-c54c1a23cb25"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""464b3a91-5662-46ab-82d3-d0d4452c90ce"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""33f6604c-f838-444d-af3a-b0abca2078f5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Broom"",
                    ""type"": ""Button"",
                    ""id"": ""b13b6cb0-a8ac-4c5e-a626-7cc4192a94b7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""0251e16a-af73-421b-b259-0a09d724feb4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Down"",
                    ""type"": ""Button"",
                    ""id"": ""3a6a3bd8-8383-4490-b027-486f95f1287f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Up"",
                    ""type"": ""Button"",
                    ""id"": ""57903629-679d-482f-8b2e-670039a06f44"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DisplayEquipment"",
                    ""type"": ""Button"",
                    ""id"": ""c84635ad-cd26-4ba4-bb24-0c8085d19ae3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cheat"",
                    ""type"": ""Button"",
                    ""id"": ""d0905a7e-bb80-4cd0-a178-baeefa27e748"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Escape"",
                    ""type"": ""Button"",
                    ""id"": ""9df5edd4-a859-4eec-80a3-8245bb5adb69"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""PS4 Dpad"",
                    ""id"": ""0a39acda-fa26-4f55-b5b6-8a50986bc4f5"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""9b7be575-8f42-4a71-8ccb-ba94441fee1f"",
                    ""path"": ""<DualShockGamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""793bef33-3c20-4cd2-8eb6-153706ee41d5"",
                    ""path"": ""<DualShockGamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b44c1691-67a3-425d-9a4e-63f66cf1e5d8"",
                    ""path"": ""<DualShockGamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b36bbbc4-6b64-43c1-956a-1ea8ab938d41"",
                    ""path"": ""<DualShockGamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""09e90be5-cef4-4179-9c71-b0ec9665ca8b"",
                    ""path"": ""<DualShockGamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""455a51b1-4dda-4e34-bd74-5346bb0da0a9"",
                    ""path"": ""<DualShockGamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b330d70d-95b0-4cf4-8dec-0a26be680da8"",
                    ""path"": ""<DualShockGamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Broom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6b291225-cbb8-497e-9368-7ea8bbe62dad"",
                    ""path"": ""<DualShockGamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Broom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7aa1ccd5-85b8-4bbd-861a-082f52e8f5b0"",
                    ""path"": ""<DualShockGamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Broom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""47e9fec3-05d2-4925-84e4-35b6704aaa92"",
                    ""path"": ""<DualShockGamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""790990b5-a1e9-49c0-9097-cc66c6a7b8f1"",
                    ""path"": ""<DualShockGamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1a7a1ac1-6567-4384-838e-651c4ac4f902"",
                    ""path"": ""<DualShockGamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DisplayEquipment"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4e9e4a54-3ce8-4f22-8171-80877d8a4223"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cf8f7e3d-bf0b-4676-b873-208d5c7f6111"",
                    ""path"": ""<DualShockGamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cheat"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ab7ab070-f394-461c-95e3-a2b503a927f5"",
                    ""path"": ""<DualShockGamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Escape"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dee5ded7-1192-4d77-b326-08a6e375ab31"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Escape"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""WASDMouse"",
            ""id"": ""e80501de-2041-4e10-8b98-26e7e9139e64"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""12a56b54-723d-4d2a-b15a-254f86f34086"",
                    ""expectedControlType"": ""Dpad"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""b953ad58-097c-48f3-a286-46e18971f720"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""c13622d9-4966-452a-a68a-b50ed6ac2697"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Broom"",
                    ""type"": ""Button"",
                    ""id"": ""ed7272b8-c230-496a-bf1f-e2f0bf4e0e6a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""6e1d6de7-a8de-430c-b08d-ed59da4432a9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Down"",
                    ""type"": ""Button"",
                    ""id"": ""1d5939d9-9288-4fa2-a881-574791978879"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Up"",
                    ""type"": ""Button"",
                    ""id"": ""bfe9b6f5-b798-40ed-aba1-bf1e0b3b8270"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DisplayEquipment"",
                    ""type"": ""Button"",
                    ""id"": ""d6f99f61-0d57-4837-bcea-a6b2621d86f3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cheat"",
                    ""type"": ""Button"",
                    ""id"": ""6da19347-3db2-409a-9307-0fb0f57a6abb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Escape"",
                    ""type"": ""Button"",
                    ""id"": ""25709f6d-c2f8-4ab9-aa16-a800f56d6c3c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Dpad"",
                    ""id"": ""a46567af-a112-4844-804f-510d26f0dd52"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""5734eb01-6bdd-4f40-b7b5-045d70c3299f"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""a00ec0c4-e3c1-4a45-8507-2e44a3e18482"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""34fea99c-56fc-4705-84f4-9044aaeda2ce"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""bb0a5862-7664-44d9-be3c-319e468d75c5"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f2ed775a-a2e0-4c54-b546-d79c5267b3bc"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e28eca6c-17de-4f33-a3df-93048d976ff1"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8bf4ae4a-f103-4744-8dd7-742397077e01"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Broom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e13532e7-cb41-4581-a7b3-2a2b92e5a1c8"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8c056faf-ac96-4050-ab51-93467582861f"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fa349e44-a1a0-480d-ac55-3da5ce79243e"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""72084626-a9cf-4e7b-96dc-008f80faa22a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""353387ef-8c8e-4280-b7d1-5652bb2f826b"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DisplayEquipment"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0b220e36-deec-4f5a-903a-49bc6c784fd8"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DisplayEquipment"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f1e0b590-5a9e-4818-aa8e-e9bb65357fed"",
                    ""path"": ""<Keyboard>/backquote"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cheat"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5ca62692-7611-4f44-9658-a33885478e73"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Escape"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Template"",
            ""id"": ""02e30d5d-064a-4c87-a7d8-5f948dd1cf1d"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""a8b706c3-07be-476b-9cff-93c1e0355749"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""7a3188e3-3aa0-4674-949c-b61dc843d21c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""1380890c-133d-4c23-bca1-467170659950"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Broom"",
                    ""type"": ""Button"",
                    ""id"": ""13b888af-26e5-4d6b-9ed8-abe22a6eaad7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""093e2613-6f62-45da-b57b-bf9e60ac0fd3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Down"",
                    ""type"": ""Button"",
                    ""id"": ""db73e26e-4aed-4c80-b4ea-f64faf6280f0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Up"",
                    ""type"": ""Button"",
                    ""id"": ""ad2ad156-3ffd-43e9-a7bc-b902a6a28429"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DisplayEquipment"",
                    ""type"": ""Button"",
                    ""id"": ""391d3d4d-6d08-4d80-81a2-d13f97865af0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cheat"",
                    ""type"": ""Button"",
                    ""id"": ""680bab41-6df7-4110-b34b-1159a278354f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Escape"",
                    ""type"": ""Button"",
                    ""id"": ""dac11f04-cdce-4682-9473-7bdea065ebb0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": []
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""New control scheme"",
            ""bindingGroup"": ""New control scheme"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""New control scheme1"",
            ""bindingGroup"": ""New control scheme1"",
            ""devices"": []
        }
    ]
}");
        // ZXCArrows
        m_ZXCArrows = asset.FindActionMap("ZXCArrows", throwIfNotFound: true);
        m_ZXCArrows_Movement = m_ZXCArrows.FindAction("Movement", throwIfNotFound: true);
        m_ZXCArrows_Jump = m_ZXCArrows.FindAction("Jump", throwIfNotFound: true);
        m_ZXCArrows_Attack = m_ZXCArrows.FindAction("Attack", throwIfNotFound: true);
        m_ZXCArrows_Broom = m_ZXCArrows.FindAction("Broom", throwIfNotFound: true);
        m_ZXCArrows_Interact = m_ZXCArrows.FindAction("Interact", throwIfNotFound: true);
        m_ZXCArrows_Down = m_ZXCArrows.FindAction("Down", throwIfNotFound: true);
        m_ZXCArrows_Up = m_ZXCArrows.FindAction("Up", throwIfNotFound: true);
        m_ZXCArrows_DisplayEquipment = m_ZXCArrows.FindAction("DisplayEquipment", throwIfNotFound: true);
        m_ZXCArrows_Cheat = m_ZXCArrows.FindAction("Cheat", throwIfNotFound: true);
        m_ZXCArrows_Escape = m_ZXCArrows.FindAction("Escape", throwIfNotFound: true);
        // PS4
        m_PS4 = asset.FindActionMap("PS4", throwIfNotFound: true);
        m_PS4_Movement = m_PS4.FindAction("Movement", throwIfNotFound: true);
        m_PS4_Jump = m_PS4.FindAction("Jump", throwIfNotFound: true);
        m_PS4_Attack = m_PS4.FindAction("Attack", throwIfNotFound: true);
        m_PS4_Broom = m_PS4.FindAction("Broom", throwIfNotFound: true);
        m_PS4_Interact = m_PS4.FindAction("Interact", throwIfNotFound: true);
        m_PS4_Down = m_PS4.FindAction("Down", throwIfNotFound: true);
        m_PS4_Up = m_PS4.FindAction("Up", throwIfNotFound: true);
        m_PS4_DisplayEquipment = m_PS4.FindAction("DisplayEquipment", throwIfNotFound: true);
        m_PS4_Cheat = m_PS4.FindAction("Cheat", throwIfNotFound: true);
        m_PS4_Escape = m_PS4.FindAction("Escape", throwIfNotFound: true);
        // WASDMouse
        m_WASDMouse = asset.FindActionMap("WASDMouse", throwIfNotFound: true);
        m_WASDMouse_Movement = m_WASDMouse.FindAction("Movement", throwIfNotFound: true);
        m_WASDMouse_Jump = m_WASDMouse.FindAction("Jump", throwIfNotFound: true);
        m_WASDMouse_Attack = m_WASDMouse.FindAction("Attack", throwIfNotFound: true);
        m_WASDMouse_Broom = m_WASDMouse.FindAction("Broom", throwIfNotFound: true);
        m_WASDMouse_Interact = m_WASDMouse.FindAction("Interact", throwIfNotFound: true);
        m_WASDMouse_Down = m_WASDMouse.FindAction("Down", throwIfNotFound: true);
        m_WASDMouse_Up = m_WASDMouse.FindAction("Up", throwIfNotFound: true);
        m_WASDMouse_DisplayEquipment = m_WASDMouse.FindAction("DisplayEquipment", throwIfNotFound: true);
        m_WASDMouse_Cheat = m_WASDMouse.FindAction("Cheat", throwIfNotFound: true);
        m_WASDMouse_Escape = m_WASDMouse.FindAction("Escape", throwIfNotFound: true);
        // Template
        m_Template = asset.FindActionMap("Template", throwIfNotFound: true);
        m_Template_Movement = m_Template.FindAction("Movement", throwIfNotFound: true);
        m_Template_Jump = m_Template.FindAction("Jump", throwIfNotFound: true);
        m_Template_Attack = m_Template.FindAction("Attack", throwIfNotFound: true);
        m_Template_Broom = m_Template.FindAction("Broom", throwIfNotFound: true);
        m_Template_Interact = m_Template.FindAction("Interact", throwIfNotFound: true);
        m_Template_Down = m_Template.FindAction("Down", throwIfNotFound: true);
        m_Template_Up = m_Template.FindAction("Up", throwIfNotFound: true);
        m_Template_DisplayEquipment = m_Template.FindAction("DisplayEquipment", throwIfNotFound: true);
        m_Template_Cheat = m_Template.FindAction("Cheat", throwIfNotFound: true);
        m_Template_Escape = m_Template.FindAction("Escape", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // ZXCArrows
    private readonly InputActionMap m_ZXCArrows;
    private IZXCArrowsActions m_ZXCArrowsActionsCallbackInterface;
    private readonly InputAction m_ZXCArrows_Movement;
    private readonly InputAction m_ZXCArrows_Jump;
    private readonly InputAction m_ZXCArrows_Attack;
    private readonly InputAction m_ZXCArrows_Broom;
    private readonly InputAction m_ZXCArrows_Interact;
    private readonly InputAction m_ZXCArrows_Down;
    private readonly InputAction m_ZXCArrows_Up;
    private readonly InputAction m_ZXCArrows_DisplayEquipment;
    private readonly InputAction m_ZXCArrows_Cheat;
    private readonly InputAction m_ZXCArrows_Escape;
    public struct ZXCArrowsActions
    {
        private @InputMaster m_Wrapper;
        public ZXCArrowsActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_ZXCArrows_Movement;
        public InputAction @Jump => m_Wrapper.m_ZXCArrows_Jump;
        public InputAction @Attack => m_Wrapper.m_ZXCArrows_Attack;
        public InputAction @Broom => m_Wrapper.m_ZXCArrows_Broom;
        public InputAction @Interact => m_Wrapper.m_ZXCArrows_Interact;
        public InputAction @Down => m_Wrapper.m_ZXCArrows_Down;
        public InputAction @Up => m_Wrapper.m_ZXCArrows_Up;
        public InputAction @DisplayEquipment => m_Wrapper.m_ZXCArrows_DisplayEquipment;
        public InputAction @Cheat => m_Wrapper.m_ZXCArrows_Cheat;
        public InputAction @Escape => m_Wrapper.m_ZXCArrows_Escape;
        public InputActionMap Get() { return m_Wrapper.m_ZXCArrows; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ZXCArrowsActions set) { return set.Get(); }
        public void SetCallbacks(IZXCArrowsActions instance)
        {
            if (m_Wrapper.m_ZXCArrowsActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnJump;
                @Attack.started -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnAttack;
                @Broom.started -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnBroom;
                @Broom.performed -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnBroom;
                @Broom.canceled -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnBroom;
                @Interact.started -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnInteract;
                @Down.started -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnDown;
                @Down.performed -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnDown;
                @Down.canceled -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnDown;
                @Up.started -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnUp;
                @Up.performed -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnUp;
                @Up.canceled -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnUp;
                @DisplayEquipment.started -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnDisplayEquipment;
                @DisplayEquipment.performed -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnDisplayEquipment;
                @DisplayEquipment.canceled -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnDisplayEquipment;
                @Cheat.started -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnCheat;
                @Cheat.performed -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnCheat;
                @Cheat.canceled -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnCheat;
                @Escape.started -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnEscape;
                @Escape.performed -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnEscape;
                @Escape.canceled -= m_Wrapper.m_ZXCArrowsActionsCallbackInterface.OnEscape;
            }
            m_Wrapper.m_ZXCArrowsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @Broom.started += instance.OnBroom;
                @Broom.performed += instance.OnBroom;
                @Broom.canceled += instance.OnBroom;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Down.started += instance.OnDown;
                @Down.performed += instance.OnDown;
                @Down.canceled += instance.OnDown;
                @Up.started += instance.OnUp;
                @Up.performed += instance.OnUp;
                @Up.canceled += instance.OnUp;
                @DisplayEquipment.started += instance.OnDisplayEquipment;
                @DisplayEquipment.performed += instance.OnDisplayEquipment;
                @DisplayEquipment.canceled += instance.OnDisplayEquipment;
                @Cheat.started += instance.OnCheat;
                @Cheat.performed += instance.OnCheat;
                @Cheat.canceled += instance.OnCheat;
                @Escape.started += instance.OnEscape;
                @Escape.performed += instance.OnEscape;
                @Escape.canceled += instance.OnEscape;
            }
        }
    }
    public ZXCArrowsActions @ZXCArrows => new ZXCArrowsActions(this);

    // PS4
    private readonly InputActionMap m_PS4;
    private IPS4Actions m_PS4ActionsCallbackInterface;
    private readonly InputAction m_PS4_Movement;
    private readonly InputAction m_PS4_Jump;
    private readonly InputAction m_PS4_Attack;
    private readonly InputAction m_PS4_Broom;
    private readonly InputAction m_PS4_Interact;
    private readonly InputAction m_PS4_Down;
    private readonly InputAction m_PS4_Up;
    private readonly InputAction m_PS4_DisplayEquipment;
    private readonly InputAction m_PS4_Cheat;
    private readonly InputAction m_PS4_Escape;
    public struct PS4Actions
    {
        private @InputMaster m_Wrapper;
        public PS4Actions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_PS4_Movement;
        public InputAction @Jump => m_Wrapper.m_PS4_Jump;
        public InputAction @Attack => m_Wrapper.m_PS4_Attack;
        public InputAction @Broom => m_Wrapper.m_PS4_Broom;
        public InputAction @Interact => m_Wrapper.m_PS4_Interact;
        public InputAction @Down => m_Wrapper.m_PS4_Down;
        public InputAction @Up => m_Wrapper.m_PS4_Up;
        public InputAction @DisplayEquipment => m_Wrapper.m_PS4_DisplayEquipment;
        public InputAction @Cheat => m_Wrapper.m_PS4_Cheat;
        public InputAction @Escape => m_Wrapper.m_PS4_Escape;
        public InputActionMap Get() { return m_Wrapper.m_PS4; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PS4Actions set) { return set.Get(); }
        public void SetCallbacks(IPS4Actions instance)
        {
            if (m_Wrapper.m_PS4ActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PS4ActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PS4ActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PS4ActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_PS4ActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PS4ActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PS4ActionsCallbackInterface.OnJump;
                @Attack.started -= m_Wrapper.m_PS4ActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_PS4ActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_PS4ActionsCallbackInterface.OnAttack;
                @Broom.started -= m_Wrapper.m_PS4ActionsCallbackInterface.OnBroom;
                @Broom.performed -= m_Wrapper.m_PS4ActionsCallbackInterface.OnBroom;
                @Broom.canceled -= m_Wrapper.m_PS4ActionsCallbackInterface.OnBroom;
                @Interact.started -= m_Wrapper.m_PS4ActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_PS4ActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_PS4ActionsCallbackInterface.OnInteract;
                @Down.started -= m_Wrapper.m_PS4ActionsCallbackInterface.OnDown;
                @Down.performed -= m_Wrapper.m_PS4ActionsCallbackInterface.OnDown;
                @Down.canceled -= m_Wrapper.m_PS4ActionsCallbackInterface.OnDown;
                @Up.started -= m_Wrapper.m_PS4ActionsCallbackInterface.OnUp;
                @Up.performed -= m_Wrapper.m_PS4ActionsCallbackInterface.OnUp;
                @Up.canceled -= m_Wrapper.m_PS4ActionsCallbackInterface.OnUp;
                @DisplayEquipment.started -= m_Wrapper.m_PS4ActionsCallbackInterface.OnDisplayEquipment;
                @DisplayEquipment.performed -= m_Wrapper.m_PS4ActionsCallbackInterface.OnDisplayEquipment;
                @DisplayEquipment.canceled -= m_Wrapper.m_PS4ActionsCallbackInterface.OnDisplayEquipment;
                @Cheat.started -= m_Wrapper.m_PS4ActionsCallbackInterface.OnCheat;
                @Cheat.performed -= m_Wrapper.m_PS4ActionsCallbackInterface.OnCheat;
                @Cheat.canceled -= m_Wrapper.m_PS4ActionsCallbackInterface.OnCheat;
                @Escape.started -= m_Wrapper.m_PS4ActionsCallbackInterface.OnEscape;
                @Escape.performed -= m_Wrapper.m_PS4ActionsCallbackInterface.OnEscape;
                @Escape.canceled -= m_Wrapper.m_PS4ActionsCallbackInterface.OnEscape;
            }
            m_Wrapper.m_PS4ActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @Broom.started += instance.OnBroom;
                @Broom.performed += instance.OnBroom;
                @Broom.canceled += instance.OnBroom;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Down.started += instance.OnDown;
                @Down.performed += instance.OnDown;
                @Down.canceled += instance.OnDown;
                @Up.started += instance.OnUp;
                @Up.performed += instance.OnUp;
                @Up.canceled += instance.OnUp;
                @DisplayEquipment.started += instance.OnDisplayEquipment;
                @DisplayEquipment.performed += instance.OnDisplayEquipment;
                @DisplayEquipment.canceled += instance.OnDisplayEquipment;
                @Cheat.started += instance.OnCheat;
                @Cheat.performed += instance.OnCheat;
                @Cheat.canceled += instance.OnCheat;
                @Escape.started += instance.OnEscape;
                @Escape.performed += instance.OnEscape;
                @Escape.canceled += instance.OnEscape;
            }
        }
    }
    public PS4Actions @PS4 => new PS4Actions(this);

    // WASDMouse
    private readonly InputActionMap m_WASDMouse;
    private IWASDMouseActions m_WASDMouseActionsCallbackInterface;
    private readonly InputAction m_WASDMouse_Movement;
    private readonly InputAction m_WASDMouse_Jump;
    private readonly InputAction m_WASDMouse_Attack;
    private readonly InputAction m_WASDMouse_Broom;
    private readonly InputAction m_WASDMouse_Interact;
    private readonly InputAction m_WASDMouse_Down;
    private readonly InputAction m_WASDMouse_Up;
    private readonly InputAction m_WASDMouse_DisplayEquipment;
    private readonly InputAction m_WASDMouse_Cheat;
    private readonly InputAction m_WASDMouse_Escape;
    public struct WASDMouseActions
    {
        private @InputMaster m_Wrapper;
        public WASDMouseActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_WASDMouse_Movement;
        public InputAction @Jump => m_Wrapper.m_WASDMouse_Jump;
        public InputAction @Attack => m_Wrapper.m_WASDMouse_Attack;
        public InputAction @Broom => m_Wrapper.m_WASDMouse_Broom;
        public InputAction @Interact => m_Wrapper.m_WASDMouse_Interact;
        public InputAction @Down => m_Wrapper.m_WASDMouse_Down;
        public InputAction @Up => m_Wrapper.m_WASDMouse_Up;
        public InputAction @DisplayEquipment => m_Wrapper.m_WASDMouse_DisplayEquipment;
        public InputAction @Cheat => m_Wrapper.m_WASDMouse_Cheat;
        public InputAction @Escape => m_Wrapper.m_WASDMouse_Escape;
        public InputActionMap Get() { return m_Wrapper.m_WASDMouse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(WASDMouseActions set) { return set.Get(); }
        public void SetCallbacks(IWASDMouseActions instance)
        {
            if (m_Wrapper.m_WASDMouseActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnJump;
                @Attack.started -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnAttack;
                @Broom.started -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnBroom;
                @Broom.performed -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnBroom;
                @Broom.canceled -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnBroom;
                @Interact.started -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnInteract;
                @Down.started -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnDown;
                @Down.performed -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnDown;
                @Down.canceled -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnDown;
                @Up.started -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnUp;
                @Up.performed -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnUp;
                @Up.canceled -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnUp;
                @DisplayEquipment.started -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnDisplayEquipment;
                @DisplayEquipment.performed -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnDisplayEquipment;
                @DisplayEquipment.canceled -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnDisplayEquipment;
                @Cheat.started -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnCheat;
                @Cheat.performed -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnCheat;
                @Cheat.canceled -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnCheat;
                @Escape.started -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnEscape;
                @Escape.performed -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnEscape;
                @Escape.canceled -= m_Wrapper.m_WASDMouseActionsCallbackInterface.OnEscape;
            }
            m_Wrapper.m_WASDMouseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @Broom.started += instance.OnBroom;
                @Broom.performed += instance.OnBroom;
                @Broom.canceled += instance.OnBroom;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Down.started += instance.OnDown;
                @Down.performed += instance.OnDown;
                @Down.canceled += instance.OnDown;
                @Up.started += instance.OnUp;
                @Up.performed += instance.OnUp;
                @Up.canceled += instance.OnUp;
                @DisplayEquipment.started += instance.OnDisplayEquipment;
                @DisplayEquipment.performed += instance.OnDisplayEquipment;
                @DisplayEquipment.canceled += instance.OnDisplayEquipment;
                @Cheat.started += instance.OnCheat;
                @Cheat.performed += instance.OnCheat;
                @Cheat.canceled += instance.OnCheat;
                @Escape.started += instance.OnEscape;
                @Escape.performed += instance.OnEscape;
                @Escape.canceled += instance.OnEscape;
            }
        }
    }
    public WASDMouseActions @WASDMouse => new WASDMouseActions(this);

    // Template
    private readonly InputActionMap m_Template;
    private ITemplateActions m_TemplateActionsCallbackInterface;
    private readonly InputAction m_Template_Movement;
    private readonly InputAction m_Template_Jump;
    private readonly InputAction m_Template_Attack;
    private readonly InputAction m_Template_Broom;
    private readonly InputAction m_Template_Interact;
    private readonly InputAction m_Template_Down;
    private readonly InputAction m_Template_Up;
    private readonly InputAction m_Template_DisplayEquipment;
    private readonly InputAction m_Template_Cheat;
    private readonly InputAction m_Template_Escape;
    public struct TemplateActions
    {
        private @InputMaster m_Wrapper;
        public TemplateActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Template_Movement;
        public InputAction @Jump => m_Wrapper.m_Template_Jump;
        public InputAction @Attack => m_Wrapper.m_Template_Attack;
        public InputAction @Broom => m_Wrapper.m_Template_Broom;
        public InputAction @Interact => m_Wrapper.m_Template_Interact;
        public InputAction @Down => m_Wrapper.m_Template_Down;
        public InputAction @Up => m_Wrapper.m_Template_Up;
        public InputAction @DisplayEquipment => m_Wrapper.m_Template_DisplayEquipment;
        public InputAction @Cheat => m_Wrapper.m_Template_Cheat;
        public InputAction @Escape => m_Wrapper.m_Template_Escape;
        public InputActionMap Get() { return m_Wrapper.m_Template; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TemplateActions set) { return set.Get(); }
        public void SetCallbacks(ITemplateActions instance)
        {
            if (m_Wrapper.m_TemplateActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_TemplateActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_TemplateActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_TemplateActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_TemplateActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_TemplateActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_TemplateActionsCallbackInterface.OnJump;
                @Attack.started -= m_Wrapper.m_TemplateActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_TemplateActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_TemplateActionsCallbackInterface.OnAttack;
                @Broom.started -= m_Wrapper.m_TemplateActionsCallbackInterface.OnBroom;
                @Broom.performed -= m_Wrapper.m_TemplateActionsCallbackInterface.OnBroom;
                @Broom.canceled -= m_Wrapper.m_TemplateActionsCallbackInterface.OnBroom;
                @Interact.started -= m_Wrapper.m_TemplateActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_TemplateActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_TemplateActionsCallbackInterface.OnInteract;
                @Down.started -= m_Wrapper.m_TemplateActionsCallbackInterface.OnDown;
                @Down.performed -= m_Wrapper.m_TemplateActionsCallbackInterface.OnDown;
                @Down.canceled -= m_Wrapper.m_TemplateActionsCallbackInterface.OnDown;
                @Up.started -= m_Wrapper.m_TemplateActionsCallbackInterface.OnUp;
                @Up.performed -= m_Wrapper.m_TemplateActionsCallbackInterface.OnUp;
                @Up.canceled -= m_Wrapper.m_TemplateActionsCallbackInterface.OnUp;
                @DisplayEquipment.started -= m_Wrapper.m_TemplateActionsCallbackInterface.OnDisplayEquipment;
                @DisplayEquipment.performed -= m_Wrapper.m_TemplateActionsCallbackInterface.OnDisplayEquipment;
                @DisplayEquipment.canceled -= m_Wrapper.m_TemplateActionsCallbackInterface.OnDisplayEquipment;
                @Cheat.started -= m_Wrapper.m_TemplateActionsCallbackInterface.OnCheat;
                @Cheat.performed -= m_Wrapper.m_TemplateActionsCallbackInterface.OnCheat;
                @Cheat.canceled -= m_Wrapper.m_TemplateActionsCallbackInterface.OnCheat;
                @Escape.started -= m_Wrapper.m_TemplateActionsCallbackInterface.OnEscape;
                @Escape.performed -= m_Wrapper.m_TemplateActionsCallbackInterface.OnEscape;
                @Escape.canceled -= m_Wrapper.m_TemplateActionsCallbackInterface.OnEscape;
            }
            m_Wrapper.m_TemplateActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @Broom.started += instance.OnBroom;
                @Broom.performed += instance.OnBroom;
                @Broom.canceled += instance.OnBroom;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Down.started += instance.OnDown;
                @Down.performed += instance.OnDown;
                @Down.canceled += instance.OnDown;
                @Up.started += instance.OnUp;
                @Up.performed += instance.OnUp;
                @Up.canceled += instance.OnUp;
                @DisplayEquipment.started += instance.OnDisplayEquipment;
                @DisplayEquipment.performed += instance.OnDisplayEquipment;
                @DisplayEquipment.canceled += instance.OnDisplayEquipment;
                @Cheat.started += instance.OnCheat;
                @Cheat.performed += instance.OnCheat;
                @Cheat.canceled += instance.OnCheat;
                @Escape.started += instance.OnEscape;
                @Escape.performed += instance.OnEscape;
                @Escape.canceled += instance.OnEscape;
            }
        }
    }
    public TemplateActions @Template => new TemplateActions(this);
    private int m_NewcontrolschemeSchemeIndex = -1;
    public InputControlScheme NewcontrolschemeScheme
    {
        get
        {
            if (m_NewcontrolschemeSchemeIndex == -1) m_NewcontrolschemeSchemeIndex = asset.FindControlSchemeIndex("New control scheme");
            return asset.controlSchemes[m_NewcontrolschemeSchemeIndex];
        }
    }
    private int m_Newcontrolscheme1SchemeIndex = -1;
    public InputControlScheme Newcontrolscheme1Scheme
    {
        get
        {
            if (m_Newcontrolscheme1SchemeIndex == -1) m_Newcontrolscheme1SchemeIndex = asset.FindControlSchemeIndex("New control scheme1");
            return asset.controlSchemes[m_Newcontrolscheme1SchemeIndex];
        }
    }
    public interface IZXCArrowsActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnBroom(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnDown(InputAction.CallbackContext context);
        void OnUp(InputAction.CallbackContext context);
        void OnDisplayEquipment(InputAction.CallbackContext context);
        void OnCheat(InputAction.CallbackContext context);
        void OnEscape(InputAction.CallbackContext context);
    }
    public interface IPS4Actions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnBroom(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnDown(InputAction.CallbackContext context);
        void OnUp(InputAction.CallbackContext context);
        void OnDisplayEquipment(InputAction.CallbackContext context);
        void OnCheat(InputAction.CallbackContext context);
        void OnEscape(InputAction.CallbackContext context);
    }
    public interface IWASDMouseActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnBroom(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnDown(InputAction.CallbackContext context);
        void OnUp(InputAction.CallbackContext context);
        void OnDisplayEquipment(InputAction.CallbackContext context);
        void OnCheat(InputAction.CallbackContext context);
        void OnEscape(InputAction.CallbackContext context);
    }
    public interface ITemplateActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnBroom(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnDown(InputAction.CallbackContext context);
        void OnUp(InputAction.CallbackContext context);
        void OnDisplayEquipment(InputAction.CallbackContext context);
        void OnCheat(InputAction.CallbackContext context);
        void OnEscape(InputAction.CallbackContext context);
    }
}
