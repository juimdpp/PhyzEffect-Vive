./ZED_Object_detection_image_viewer.exe .\sample-data\ball1_hard.svo2 .\MLForSys\ball1_hard\output\ball1_hard_full_annotations.json  .\MLForSys\ball1_hard\output\ball1_hard_centroids.txt
Write-Host "Transformed ball1_hard"
./ZED_Object_detection_image_viewer.exe .\sample-data\ball1_soft.svo2 .\MLForSys\ball1_soft\output\ball1_soft_full_annotations.json  .\MLForSys\ball1_soft\output\ball1_soft_centroids.txt
Write-Host "Transformed ball1_soft"

./ZED_Object_detection_image_viewer.exe .\sample-data\ball2_hard.svo2 .\MLForSys\ball2_hard\output\ball2_hard_full_annotations.json  .\MLForSys\ball2_hard\output\ball2_hard_centroids.txt
Write-Host "Transformed ball2_hard"
./ZED_Object_detection_image_viewer.exe .\sample-data\ball2_soft.svo2 .\MLForSys\ball2_soft\output\ball2_soft_full_annotations.json  .\MLForSys\ball2_soft\output\ball2_soft_centroids.txt
Write-Host "Transformed ball2_soft"
