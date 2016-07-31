var ImageOrganizer; if (!ImageOrganizer) ImageOrganizer = {};

ImageOrganizer.Launch = function(carID, version) {
window.open("/ImageOrganizerLauncher.html?AdminUrl=http://" + window.location.href.split('/')[2] + "&Url=http://gear.co.il:1387/Gear/Content/Images&ImageView=gear_ModelImage&ObjectView=gear_Model&controller=GearImageOrganizer&GetFolderImages=GetFolderImages&GetRowImages=GetRowImages&GetRowImages=GetRowImages&GetRows=GetRows&GetUploadFolders=GetUploadFolders&SetRowImages=SetRowImages&SelectedObjectKey=" + carID + "," + version, 'ImageOrganizer');
}