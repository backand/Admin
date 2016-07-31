var ImageOrganizer; if (!ImageOrganizer) ImageOrganizer = {};

ImageOrganizer.Launch = function(carID, version) {
window.open("/GearAdmin/ImageOrganizerLauncher.html?AdminUrl=http://" + window.location.href.split('/')[2] + "/GearAdmin&Url=http://durados.info/Gear/Content/Images&ImageView=gear_ModelImage&ObjectView=gear_Model&controller=GearImageOrganizer&GetFolderImages=GetFolderImages&GetRowImages=GetRowImages&GetRowImages=GetRowImages&GetRows=GetRows&GetUploadFolders=GetUploadFolders&SetRowImages=SetRowImages&SelectedObjectKey=" + carID + "," + version, 'ImageOrganizer');
}