// Enum for Gear
namespace Durados.Web.Mvc.Specifics.Projects.Gear {
    using System;
    
    
    public enum GearViews {
        
        gear_User,
        
        gear_AreaCode,
        
        gear_City,
        
        gear_Article,
        
        gear_UpholsteryType,
        
        gear_ArticleCategory,
        
        gear_ArticleCategoryRel,
        
        gear_ArticleGroup,
        
        gear_ArticleProducer,
        
        gear_BrakeType,
        
        gear_Car,
        
        gear_Category,
        
        gear_CategoryGroup,
        
        gear_ClimateControlSystemType,
        
        gear_DriveType,
        
        gear_EngineConfiguration,
        
        gear_EngineFuelInjectionType,
        
        gear_EngineLayout,
        
        gear_EngineType,
        
        gear_EntertainmentSystemType,
        
        gear_GearboxType,
        
        gear_Group,
        
        gear_InsideDecorationType,
        
        gear_Model,
        
        gear_ModelContent,
        
        gear_ModelImage,
        
        gear_Price,
        
        gear_Producer,
        
        gear_Range,
        
        gear_SpareWheelType,
        
        gear_SteeringType,
        
        durados_Contact,
        
        durados_TemplateScript,
        
        durados_Content,
        
        durados_ContentParameter,
        
        durados_ContentType,
        
        durados_ContentTypeCss,
        
        durados_ContentTypeScript,
        
        durados_Css,
        
        durados_Field,
        
        durados_FieldCategory,
        
        durados_FieldCategoryRel,
        
        durados_Form,
        
        durados_FormType,
        
        durados_Frame,
        
        durados_Html,
        
        durados_Image,
        
        durados_Link,
        
        durados_LinkParameter,
        
        durados_Menu,
        
        durados_MenuItem,
        
        durados_Object,
        
        durados_Page,
        
        durados_PageContent,
        
        durados_Parameter,
        
        durados_PlaceHolder,
        
        durados_Script,
        
        durados_Template,
        
        durados_TemplateCss,
        
        durados_TemplatePlaceHolder,
        
        gear_CarSafety,
        
        gear_Galery,
    }
    
    public enum gear_User {
        
        FK_gear_User_gear_AreaCode_Parent,
        
        FK_gear_User_gear_AreaCode1_Parent,
        
        FK_gear_User_gear_City_Parent,
        
        ID,
        
        Email,
        
        Password,
        
        ConfirmPassword,
        
        FirstName,
        
        LastName,
        
        Street,
        
        House,
        
        Apartment,
        
        Phone1,
        
        Phone2,
        
        BirthDate,
        
        Newsletters,
        
        ApproveRegulations,
        
        OldAndNew,
    }
    
    public enum gear_AreaCode {
        
        Prefix,
        
        Name,
        
        FK_gear_User_gear_AreaCode_Children,
        
        FK_gear_User_gear_AreaCode1_Children,
    }
    
    public enum gear_City {
        
        ID,
        
        Name,
        
        FK_gear_User_gear_City_Children,
    }
    
    public enum gear_Article {
        
        ID,
        
        Title,
        
        Header,
        
        SecondaryHeader,
        
        Body,
        
        Picture,
        
        CreationDate,
        
        PublishFrom,
        
        PublishTo,
        
        Approved,
        
        Selected,
        
        FK_gear_ArticleCategoryRel_gear_Article_Children,
        
        FK_gear_ArticleGroup_gear_Article_Children,
        
        FK_gear_ArticleProducer_gear_Article_Children,
    }
    
    public enum gear_UpholsteryType {
        
        ID,
        
        NAME,
        
        FK_gear_Car_gear_UpholsteryType_Children,
    }
    
    public enum gear_ArticleCategory {
        
        ID,
        
        Name,
        
        FK_gear_ArticleCategoryRel_gear_ArticleCategory_Children,
    }
    
    public enum gear_ArticleCategoryRel {
        
        FK_gear_ArticleCategoryRel_gear_Article_Parent,
        
        FK_gear_ArticleCategoryRel_gear_ArticleCategory_Parent,
        
        ID,
    }
    
    public enum gear_ArticleGroup {
        
        FK_gear_ArticleGroup_gear_Article_Parent,
        
        FK_gear_ArticleGroup_gear_Group_Parent,
        
        ID,
    }
    
    public enum gear_ArticleProducer {
        
        FK_gear_ArticleProducer_gear_Article_Parent,
        
        FK_gear_ArticleProducer_gear_Producer_Parent,
        
        ID,
    }
    
    public enum gear_BrakeType {
        
        ID,
        
        NAME,
        
        FK_gear_Car_gear_BrakeType_Children,
        
        FK_gear_Car_gear_BrakeType1_Children,
    }
    
    public enum gear_Car {
        
        FK_gear_Car_gear_BrakeType_Parent,
        
        FK_gear_Car_gear_BrakeType1_Parent,
        
        FK_gear_Car_gear_CarCategory_Parent,
        
        FK_gear_Car_gear_CarProducer_Parent,
        
        FK_gear_Car_gear_ClimateControlSystemType_Parent,
        
        FK_gear_Car_gear_DriveType_Parent,
        
        FK_gear_Car_gear_EngineConfiguration_Parent,
        
        FK_gear_Car_gear_EngineFuelInjectionType_Parent,
        
        FK_gear_Car_gear_EngineLayout_Parent,
        
        FK_gear_Car_gear_EngineType_Parent,
        
        FK_gear_Car_gear_EntertainmentSystemType_Parent,
        
        FK_gear_Car_gear_GearboxType_Parent,
        
        FK_gear_Car_gear_InsideDecorationType_Parent,
        
        FK_gear_Car_gear_SpareWheelType_Parent,
        
        FK_gear_Car_gear_SteeringType_Parent,
        
        FK_gear_Car_gear_UpholsteryType_Parent,
        
        ID,
        
        CarID,
        
        VERSION,
        
        VARIANT,
        
        DISPLAY,
        
        LAST_UPDATE_TIME,
        
        LAST_UPDATE_ADMIN_NAME,
        
        NAME,
        
        MODEL_NAME,
        
        PRICE,
        
        Reduction,
        
        LICENSE_GROUP,
        
        WORLD_LAUNCH_YEAR,
        
        FIRST_LOCAL_YEAR,
        
        LAST_LOCAL_YEAR,
        
        AVAILABLE_FOR_TEST_DRIVE,
        
        LICENSING_CODES,
        
        PRICE_LIST_CODE,
        
        DOOR_NUMBER,
        
        WARRANTY_KM,
        
        WARRANTY_PERIOD_IN_MONTH,
        
        SERVICE_SCHEDULE_KM,
        
        SERVICE_SCHEDULE_PERIOD_IN_MONTH,
        
        REMARKS,
        
        CAR_IMAGE_PATH,
        
        ADDITIONAL_EQUIPMENT,
        
        OPTIONAL_EQUIPMENT,
        
        ENGINE_TURBO,
        
        ENGINE_CO2_EMISSIONS_IN_GRAM_PER_KM,
        
        ENGINE_CO_EMISSIONS_IN_GRAM_PER_KM,
        
        ENGINE_HC_EMISSIONS_IN_GRAM_PER_KM,
        
        ENGINE_NOX_EMISSIONS_IN_GRAM_PER_KM,
        
        ENGINE_PM_EMISSIONS_IN_GRAM_PER_KM,
        
        GREEN_GRADE,
        
        POLLUTION_GRADE,
        
        ENGINE_VOLUME_CC,
        
        ENGINE_CYLINDER_NUMBER,
        
        ENGINE_VALVE_NUMBER,
        
        ENGINE_MAX_POWER_HP,
        
        ENGINE_MAX_POWER_RPM,
        
        ENGINE_MAX_TORQUE_KGM,
        
        ENGINE_MAX_TORQUE_RPM,
        
        ELECTRIC_FRONT_ENGINE,
        
        ELECTRIC_FRONT_ENGINE_MAX_POWER_HP,
        
        ELECTRIC_FRONT_ENGINE_MAX_POWER_RPM,
        
        ELECTRIC_FRONT_ENGINE_MAX_TORQUE_KGM,
        
        ELECTRIC_FRONT_ENGINE_MAX_TORQUE_RPM,
        
        ELECTRIC_REAR_ENGINE,
        
        ELECTRIC_REAR_ENGINE_MAX_POWER_HP,
        
        ELECTRIC_REAR_ENGINE_MAX_POWER_RPM,
        
        ELECTRIC_REAR_ENGINE_MAX_TORQUE_KGM,
        
        ELECTRIC_REAR_ENGINE_MAX_TORQUE_RPM,
        
        SAFETY_TEST_NEW,
        
        SAFETY_TEST_YEAR,
        
        SAFETY_TEST_ADULT_OCCUPANT_PERCENTAGE,
        
        SAFETY_TEST_CHILD_OCCUPANT_PERCENTAGE,
        
        SAFETY_TEST_PEDESTRAIN_PERCENTAGE,
        
        SAFETY_TEST_SAFETY_ASSIST_PERCENTAGE,
        
        SAFETY_TEST_SAFETY_OVERALL_POINTS,
        
        SAFETY_TEST_ADULT_OCCUPANT_POINTS,
        
        SAFETY_TEST_CHILD_OCCUPANT_POINTS,
        
        SAFETY_TEST_PEDESTRAIN_POINTS,
        
        SAFETY_AIR_BAG_NUMBER,
        
        SAFETY_FRONT_PASSENGER_AIR_BAG_DISABLE,
        
        SAFETY_ISOFIX,
        
        SAFETY_ABS,
        
        SAFETY_BAS,
        
        SAFETY_EBD,
        
        SAFETY_ESP,
        
        SAFETY_TCS,
        
        SAFETY_ASR,
        
        SAFETY_EDL,
        
        SAFETY_HDC,
        
        SAFETY_SLS,
        
        SAFETY_RSC,
        
        SAFETY_CBC,
        
        SAFETY_ARP,
        
        SAFETY_IMMOBILIZER,
        
        SAFETY_ALARM,
        
        SAFETY_FOG_LIGHTS,
        
        SAFETY_XENON_HEADLIGHTS,
        
        SAFETY_HEADLIGHTS_CLEANERS,
        
        SAFETY_ADAPTIVE_HEADLIGHTS,
        
        SAFETY_AUTO_DIMMING_REAR_VIEW_MIRROR,
        
        SAFETY_DUSK_SENSOR_WITH_AUTOMATIC_DRIVING_LIGHTS,
        
        SAFETY_RAIN_SENSOR_WITH_AUTOMATIC_WINDSCREEN_WIPERS,
        
        SAFETY_PRE_CRASH_SYSTEM,
        
        DIMENSION_LENGTH_IN_MILLIMETERS,
        
        DIMENSION_WIDTH_IN_MILLIMETERS,
        
        DIMENSION_HEIGHT_IN_MILLIMETERS,
        
        DIMENSION_WHEELBASE_IN_MILLIMETERS,
        
        DIMENSION_TURNING_CIRCLE_IN_MILLIMETERS,
        
        DIMENSION_FRONT_HEADROOM_IN_MILLIMETERS,
        
        DIMENSION_REAR_HEADROOM_IN_MILLIMETERS,
        
        DIMENSION_FRONT_SHOULDER_WIDTH_IN_MILLIMETERS,
        
        DIMENSION_REAR_SHOULDER_WIDTH_IN_MILLIMETERS,
        
        DIMENSION_FRONT_LEGS_LENGTH_IN_MILLIMETERS,
        
        DIMENSION_REAR_LEGS_LENGTH_IN_MILLIMETERS,
        
        DIMENSION_KERB_WEIGHT_IN_KILOGRAMS,
        
        DIMENSION_GROSS_WEIGHT_IN_KILOGRAMS,
        
        DIMENSION_FUEL_TANK_CAPACITY_IN_LITRES,
        
        DIMENSION_TRUNK_CAPACITY_IN_LITRES,
        
        DIMENSION_TRUNK_FOLDED_SEAT_CAPACITY_IN_LITRES,
        
        DIMENSION_REMARKS,
        
        WHEELS_FRONT_TIRE_WIDTH_IN_MILLIMETERS,
        
        WHEELS_FRONT_TIRE_ASPECT_RATIO_IN_PERCENT,
        
        WHEELS_FRONT_TIRE_DIAMETER_IN_INCHES,
        
        WHEELS_REAR_TIRE_WIDTH_IN_MILLIMETERS,
        
        WHEELS_REAR_TIRE_ASPECT_RATIO_IN_PERCENT,
        
        WHEELS_REAR_TIRE_DIAMETER_IN_INCHES,
        
        ALLOY_WHEELS,
        
        TOP_SPEED_IN_KM_PER_HOUR,
        
        TOP_SPEED_STRING,
        
        ACCELERATION_ZERO_TO_HUNDRED_IN_SECONDS,
        
        ACCELERATION_STRING,
        
        FUEL_CONSUMPTION_CITY_DRIVE_IN_KM_PER_LITRE,
        
        FUEL_CONSUMPTION_HIGHWAY_DRIVE_IN_KM_PER_LITRE,
        
        FUEL_CONSUMPTION_COMBINED_DRIVE_IN_KM_PER_LITRE,
        
        GEARBOX_GEAR_NUMBER,
        
        GEARBOX_TIPTRONIC_MODE,
        
        GEARBOX_SPORT_MODE,
        
        GEARBOX_SNOW_MODE,
        
        GEARBOX_SAVE_MODE,
        
        EQUIPMENT_LEATHER_COATED_STEERING_WHEEL,
        
        EQUIPMENT_STEERING_WHEEL_CONTROL,
        
        EQUIPMENT_STEERING_WHEEL_TILT_ADJUSTMENT,
        
        EQUIPMENT_STEERING_WHEEL_TELESCOPIC_ADJUSTMENT,
        
        EQUIPMENT_STEERING_WHEEL_ELECTRONIC_ADJUSTMENT,
        
        EQUIPMENT_STEERING_WHEEL_AUTOMATIC_MEMORY_ADJUSTMENT,
        
        SUSPENSION_FRONT_TYPE,
        
        SUSPENSION_REAR_TYPE,
        
        EQUIPMENT_ELECTRIC_DOOR_MIRRORS,
        
        EQUIPMENT_AUTOMATIC_MEMORY_ADJUSTMENT_DOOR_MIRRORS,
        
        EQUIPMENT_ELECTRIC_HEATED_DOOR_MIRRORS,
        
        EQUIPMENT_ELECTRIC_FOLDED_DOORM_IRRORS,
        
        EQUIPMENT_AUTO_DIMMING_DOOR_MIRRORS,
        
        EQUIPMENT_INTEGRATED_INDICATORS_IN_DOOR_MIRRORS,
        
        SEAT_NUMBER,
        
        EQUIPMENT_SPORT_SEATS,
        
        EQUIPMENT_HEATED_FRONT_SEATS,
        
        EQUIPMENT_HEATED_REAR_SEATS,
        
        EQUIPMENT_HEATED_AND_AIR_CONDITIONED_FRONT_SEATS,
        
        EQUIPMENT_HEATED_AND_AIR_CONDITIONED_REAR_SEATS,
        
        EQUIPMENT_ELECTRONIC_ADJUSTMENT_DRIVER_SEAT,
        
        EQUIPMENT_ELECTRONIC_ADJUSTMENT_FRONT_PASSENGER_SEAT,
        
        EQUIPMENT_AUTOMATIC_MEMORY_ADJUSTMENT_DRIVER_SEAT,
        
        EQUIPMENT_AUTOMATIC_MEMORY_ADJUSTMENT_FRONT_PASSENGER_SEAT,
        
        EQUIPMENT_LOUDSPEAKER_NUMBER,
        
        EQUIPMENT_PHONE_OR_BLUETOOTH_SPEAKER,
        
        EQUIPMENT_GPS,
        
        ELECTRONIC_PARKING_BRAKE,
        
        EQUIPMENT_LEATHER_COATED_PARKING_BRAKE,
        
        SAFETY_PARKING_ASSIST_SYSTEM,
        
        SAFETY_FRONT_PARKING_SENSOR,
        
        SAFETY_REAR_PARKING_SENSOR,
        
        SAFETY_REAR_PARKING_CAMERA,
        
        EQUIPMENT_ELECTRIC_WINDOW_NUMBER,
        
        EQUIPMENT_CENTRAL_LOCKING,
        
        EQUIPMENT_SUNROOF,
        
        EQUIPMENT_ROUTE_COMPUTER,
        
        EQUIPMENT_CRUISE_CONTROL,
        
        EQUIPMENT_FRONT_READING_LIGHTS,
        
        EQUIPMENT_REAR_READING_LIGHTS,
        
        EQUIPMENT_12V_SOCKETS_NUMBER,
        
        EQUIPMENT_ELECTRIC_REAR_SUNSHADE,
        
        OFF_ROAD_CAR,
        
        GROUND_CLEARANCE_OFF_ROAD_MODE_IN_MILLIMETERS,
        
        APPROACH_ANGLE_OFF_ROAD_MODE,
        
        RAMP_BREAKOVER_ANGLE_OFF_ROAD_MODE,
        
        DEPARTURE_ANGLE_OFF_ROAD_MODE,
        
        WADING_DEPTH_OFF_ROAD_MODE_IN_MILLIMETERS,
        
        FK_gear_Price_gear_Car_Children,
    }
    
    public enum gear_Category {
        
        ID,
        
        NAME,
        
        FK_gear_Car_gear_CarCategory_Children,
        
        FK_gear_CategoryGroup_gear_Category_Children,
        
        FK_gear_Model_gear_Category_Children,
        
        gear_Category_gear_CarSafety_Children,
    }
    
    public enum gear_CategoryGroup {
        
        FK_gear_CategoryGroup_gear_Category_Parent,
        
        FK_gear_CategoryGroup_gear_Group_Parent,
        
        ID,
        
        SEOKeys,
    }
    
    public enum gear_ClimateControlSystemType {
        
        ID,
        
        NAME,
        
        FK_gear_Car_gear_ClimateControlSystemType_Children,
    }
    
    public enum gear_DriveType {
        
        ID,
        
        NAME,
        
        FK_gear_Car_gear_DriveType_Children,
    }
    
    public enum gear_EngineConfiguration {
        
        ID,
        
        NAME,
        
        FK_gear_Car_gear_EngineConfiguration_Children,
    }
    
    public enum gear_EngineFuelInjectionType {
        
        ID,
        
        NAME,
        
        FK_gear_Car_gear_EngineFuelInjectionType_Children,
    }
    
    public enum gear_EngineLayout {
        
        ID,
        
        NAME,
        
        FK_gear_Car_gear_EngineLayout_Children,
    }
    
    public enum gear_EngineType {
        
        ID,
        
        NAME,
        
        FK_gear_Car_gear_EngineType_Children,
    }
    
    public enum gear_EntertainmentSystemType {
        
        ID,
        
        NAME,
        
        FK_gear_Car_gear_EntertainmentSystemType_Children,
    }
    
    public enum gear_GearboxType {
        
        ID,
        
        NAME,
        
        FK_gear_Car_gear_GearboxType_Children,
    }
    
    public enum gear_Group {
        
        ID,
        
        Name,
        
        Logo,
        
        FK_gear_ArticleGroup_gear_Group_Children,
        
        FK_gear_CategoryGroup_gear_Group_Children,
    }
    
    public enum gear_InsideDecorationType {
        
        ID,
        
        NAME,
        
        FK_gear_Car_gear_InsideDecorationType_Children,
    }
    
    public enum gear_Model {
        
        FK_gear_Model_gear_Category_Parent,
        
        FK_gear_Model_gear_Producer_Parent,
        
        CarID,
        
        VERSION,
        
        MODEL_NAME,
        
        MinPrice,
        
        MaxPrice,
        
        Reduction,
        
        FirstYear,
        
        LastYear,
        
        AVAILABLE_FOR_TEST_DRIVE,
        
        FK_gear_ModelContent_gear_Model_Children,
        
        FK_gear_ModelImage_gear_Model_Children,
    }
    
    public enum gear_ModelContent {
        
        FK_gear_ModelContent_gear_Model_Parent,
        
        ID,
        
        Name,
        
        ContentID,
    }
    
    public enum gear_ModelImage {
        
        FK_gear_ModelImage_gear_Model_Parent,
        
        FK_gear_ModelImage_gear_Galery_Parent,
        
        ID,
        
        Image,
        
        Ordinal,
    }
    
    public enum gear_Price {
        
        FK_gear_Price_gear_Car_Parent,
        
        ID,
        
        Year,
        
        Price,
        
        MonthAddition,
        
        New,
        
        LastUpdated,
        
        UpdatedBy,
    }
    
    public enum gear_Producer {
        
        ID,
        
        NAME,
        
        ENGLISH_NAME,
        
        ACTIVE_IN_LOCAL_MARKET,
        
        LOGO,
        
        FK_gear_ArticleProducer_gear_Producer_Children,
        
        FK_gear_Car_gear_CarProducer_Children,
        
        FK_gear_Model_gear_Producer_Children,
        
        gear_Producer_gear_CarSafety_Children,
    }
    
    public enum gear_Range {
        
        ID,
        
        Name,
        
        Min,
        
        Max,
        
        Step,
        
        DefaultMinValue,
        
        DefaultMaxValue,
        
        MinLabel,
        
        MaxLabel,
        
        Under,
        
        Over,
        
        UnderMinLabel,
        
        OverMaxLabel,
        
        MappedColumn,
    }
    
    public enum gear_SpareWheelType {
        
        ID,
        
        NAME,
        
        FK_gear_Car_gear_SpareWheelType_Children,
    }
    
    public enum gear_SteeringType {
        
        ID,
        
        NAME,
        
        FK_gear_Car_gear_SteeringType_Children,
    }
    
    public enum durados_Contact {
        
        ID,
        
        Name,
        
        Subject,
        
        To,
        
        CC,
        
        SMTP,
        
        Title,
        
        Header,
        
        Footer,
        
        FromRequired,
        
        AnonymousEmail,
    }
    
    public enum durados_TemplateScript {
        
        FK_TemplateScript_Script_Parent,
        
        FK_TemplateScript_Template_Parent,
        
        ID,
    }
    
    public enum durados_Content {
        
        FK_Content_ContentType_Parent,
        
        FK_durados_Content_durados_Frame_Parent,
        
        FK_durados_Content_durados_Html_Parent,
        
        FK_durados_Content_durados_Image_Parent,
        
        ID,
        
        Name,
        
        CustomTypeID,
        
        FK_ContentParameter_Content_Children,
        
        FK_PageContent_Content_Children,
    }
    
    public enum durados_ContentParameter {
        
        FK_ContentParameter_Content_Parent,
        
        FK_ContentParameter_Parameter_Parent,
        
        ID,
    }
    
    public enum durados_ContentType {
        
        ID,
        
        Name,
        
        Custom,
        
        FK_Content_ContentType_Children,
        
        FK_ContentTypeCss_ContentType_Children,
        
        FK_ContentTypeScript_ContentType_Children,
    }
    
    public enum durados_ContentTypeCss {
        
        FK_ContentTypeCss_ContentType_Parent,
        
        FK_ContentTypeCss_Css_Parent,
        
        ID,
    }
    
    public enum durados_ContentTypeScript {
        
        FK_ContentTypeScript_ContentType_Parent,
        
        FK_ContentTypeScript_Script_Parent,
        
        ID,
        
        Ordinal,
    }
    
    public enum durados_Css {
        
        ID,
        
        Name,
        
        Reference,
        
        FK_ContentTypeCss_Css_Children,
        
        FK_TemplateCss_Css_Children,
    }
    
    public enum durados_Field {
        
        FK_durados_Field_durados_Object_Parent,
        
        ID,
        
        Name,
        
        DisplayName,
        
        Format,
        
        RelatedName,
        
        BetterIsGreater,
        
        Roles,
        
        FK_durados_FieldCategoryRel_durados_Field_Children,
    }
    
    public enum durados_FieldCategory {
        
        FK_durados_FieldCategory_durados_Object_Parent,
        
        ID,
        
        Name,
        
        DisplayName,
        
        FK_durados_FieldCategoryRel_durados_FieldCategory_Children,
    }
    
    public enum durados_FieldCategoryRel {
        
        FK_durados_FieldCategoryRel_durados_Field_Parent,
        
        FK_durados_FieldCategoryRel_durados_FieldCategory_Parent,
        
        ID,
        
        Ordinal,
    }
    
    public enum durados_Form {
        
        FK_durados_Form_durados_FormType_Parent,
        
        ID,
        
        Name,
        
        ViewName,
        
        DataAction,
    }
    
    public enum durados_FormType {
        
        ID,
        
        Name,
        
        FK_durados_Form_durados_FormType_Children,
    }
    
    public enum durados_Frame {
        
        ID,
        
        Name,
        
        Url,
        
        FK_durados_Content_durados_Frame_Children,
    }
    
    public enum durados_Html {
        
        ID,
        
        Name,
        
        Text,
        
        FK_durados_Content_durados_Html_Children,
    }
    
    public enum durados_Image {
        
        ID,
        
        Name,
        
        Src,
        
        ToolTip,
        
        FK_durados_Content_durados_Image_Children,
    }
    
    public enum durados_Link {
        
        FK_Link_Page_Parent,
        
        ID,
        
        Name,
        
        Url,
        
        Action,
        
        Controller,
        
        Script,
        
        Target,
        
        LoginRequired,
        
        Text,
        
        Tooltip,
        
        ControlName,
        
        FK_LinkParameter_Link_Children,
        
        FK_MenuItem_Link_Children,
    }
    
    public enum durados_LinkParameter {
        
        FK_LinkParameter_Link_Parent,
        
        FK_LinkParameter_Parameter_Parent,
        
        ID,
    }
    
    public enum durados_Menu {
        
        ID,
        
        Name,
        
        Orientation,
        
        FK_MenuItem_Menu_Children,
    }
    
    public enum durados_MenuItem {
        
        FK_MenuItem_Link_Parent,
        
        FK_MenuItem_Menu_Parent,
        
        FK_MenuItem_MenuItem_Parent,
        
        ID,
        
        Ordinal,
        
        FK_MenuItem_MenuItem_Children,
    }
    
    public enum durados_Object {
        
        ID,
        
        Name,
        
        FullName,
        
        DisplayName,
        
        FK_durados_Field_durados_Object_Children,
        
        FK_durados_FieldCategory_durados_Object_Children,
    }
    
    public enum durados_Page {
        
        FK_Page_Master_Parent,
        
        ID,
        
        Name,
        
        Title,
        
        FK_Link_Page_Children,
        
        FK_PageContent_Page_Children,
    }
    
    public enum durados_PageContent {
        
        FK_PageContent_Content_Parent,
        
        FK_PageContent_Page_Parent,
        
        FK_PageContent_PlaceHolder_Parent,
        
        ID,
        
        Ordinal,
    }
    
    public enum durados_Parameter {
        
        ID,
        
        Name,
        
        Value,
        
        FK_ContentParameter_Parameter_Children,
        
        FK_LinkParameter_Parameter_Children,
    }
    
    public enum durados_PlaceHolder {
        
        ID,
        
        Name,
        
        FK_PageContent_PlaceHolder_Children,
        
        FK_TemplatePlaceHolder_PlaceHolder_Children,
    }
    
    public enum durados_Script {
        
        ID,
        
        Name,
        
        Reference,
        
        FK_TemplateScript_Script_Children,
        
        FK_ContentTypeScript_Script_Children,
    }
    
    public enum durados_Template {
        
        ID,
        
        Name,
        
        FK_TemplateScript_Template_Children,
        
        FK_Page_Master_Children,
        
        FK_TemplateCss_Template_Children,
        
        FK_TemplatePlaceHolder_Template_Children,
    }
    
    public enum durados_TemplateCss {
        
        FK_TemplateCss_Css_Parent,
        
        FK_TemplateCss_Template_Parent,
        
        ID,
    }
    
    public enum durados_TemplatePlaceHolder {
        
        FK_TemplatePlaceHolder_PlaceHolder_Parent,
        
        FK_TemplatePlaceHolder_Template_Parent,
        
        ID,
    }
    
    public enum gear_CarSafety {
        
        gear_Producer_gear_CarSafety_Parent,
        
        gear_Category_gear_CarSafety_Parent,
        
        ID,
        
        CarID,
        
        VERSION,
        
        VARIANT,
        
        NAME,
        
        SAFETY_TEST_NEW,
        
        SAFETY_TEST_YEAR,
        
        SAFETY_TEST_ADULT_OCCUPANT_PERCENTAGE,
        
        SAFETY_TEST_CHILD_OCCUPANT_PERCENTAGE,
        
        SAFETY_TEST_PEDESTRAIN_PERCENTAGE,
        
        SAFETY_TEST_SAFETY_ASSIST_PERCENTAGE,
        
        SAFETY_TEST_SAFETY_OVERALL_POINTS,
        
        SAFETY_TEST_ADULT_OCCUPANT_POINTS,
        
        SAFETY_TEST_CHILD_OCCUPANT_POINTS,
        
        SAFETY_TEST_PEDESTRAIN_POINTS,
        
        SAFETY_AIR_BAG_NUMBER,
        
        SAFETY_FRONT_PASSENGER_AIR_BAG_DISABLE,
        
        SAFETY_ISOFIX,
        
        SAFETY_ABS,
        
        SAFETY_BAS,
        
        SAFETY_EBD,
        
        SAFETY_ESP,
        
        SAFETY_TCS,
        
        SAFETY_ASR,
        
        SAFETY_EDL,
        
        SAFETY_HDC,
        
        SAFETY_SLS,
        
        SAFETY_RSC,
        
        SAFETY_CBC,
        
        SAFETY_ARP,
        
        SAFETY_IMMOBILIZER,
        
        SAFETY_ALARM,
        
        SAFETY_FOG_LIGHTS,
        
        SAFETY_XENON_HEADLIGHTS,
        
        SAFETY_HEADLIGHTS_CLEANERS,
        
        SAFETY_ADAPTIVE_HEADLIGHTS,
        
        SAFETY_AUTO_DIMMING_REAR_VIEW_MIRROR,
        
        SAFETY_DUSK_SENSOR_WITH_AUTOMATIC_DRIVING_LIGHTS,
        
        SAFETY_RAIN_SENSOR_WITH_AUTOMATIC_WINDSCREEN_WIPERS,
        
        SAFETY_PRE_CRASH_SYSTEM,
    }
    
    public enum gear_Galery {
        
        ID,
        
        Name,
        
        Image,
        
        Ordinal,
        
        FK_gear_ModelImage_gear_Galery_Children,
    }
}
