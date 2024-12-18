# PhyzEffect-Vive

Link to GitHub repository: [PhyzEffect-Vive](https://github.com/juimdpp/PhyzEffect-Vive)

---

## Steps to Set Up and Use

Follow the above steps to extract and simulate the virtual trajectory.

1. **Setup ZED Camera.**

2. **Setup Scene.**
   - Print at least 3 ArUco markers and place them in the scene.
   - Ensure the markers have different x, y, z positions so the bounding box created by them surrounds the entire scene in 3D.

3. **Scan the Real World.**
   - Use Scaniverse to scan the scene.
   - Ensure the ArUco markers are clearly visible in the scan.

4. **Record Video of Interacting Object.**
   - *(TODO: Additional GUI needed to make this feature accessible to end-users.)*
   - Use the Unity app provided in the repository.
     - Set the file directory for saving the recording.
     - Press "Start recording," perform the interaction, and ensure the camera remains stationary.
     - Press "Stop recording."
   - **Note:** Sample recordings used in the final results are included.

5. **Export SVO Video into RGB + Depth PNG Images.**
   - Use the executable provided by ZED SDK: [ZED SDK Export Tool](https://github.com/stereolabs/zed-sdk/tree/master/recording/export/svo/cpp).
   - **Run Command:**

     ```bash
     ./ZED_SVO_Export.exe "path/to/file.svo" "path/to/output/folder"
     ```

   - Converted images for sample recordings are included.

6. **Analyze Recordings to Get Bounding Boxes.**
   - Use the Python script for analysis. Manually annotate frames where the ball is not detected.
     - Install dependencies with `pip install -r requirements.txt`.
     - Run `3d-annotate.ipynb` and modify the `base_dir` for each video.
     - The script detects the ball and prompts for manual annotation if needed.
     - Outputs 3 files, but only `base_dir_full_annotation.json` is important.
   - **Note:** Annotated data for sample recordings is included.

7. **Convert Bounding Boxes to Center Points and Camera Coordinates.**
   - Run the executable:

     ```bash
     ZED_Object_detection_image_viewer.exe <svo> <inputfile> <outputfile>
     ```

   - The executable extracts the centroid of the detected ball using bounding boxes and saves them in camera coordinates.
   - **Note:** Instructions for building the executable are available in the [ZED SDK documentation](https://www.stereolabs.com/docs/app-development/cpp/windows). A script to run the sample data is included.

8. **Run the Unity Project.**
   - Use Unity 2022.3.21f1 and ensure ZED SDK is installed.
   - Open the `MLForSys` scene. The `Optimizer` object has 3 attached scripts:
     - **`SystemManager.cs`**: Manages the overall procedure, GUIs, and data flow between systems.
     - **`RealSystem.cs`**: Processes real-world trajectory data, transforming centroids from camera coordinates to world space using the ZED Mini camera.
     - **`VirtualSystem.cs`**: Simulates the virtual ball trajectory.
   - Provide the path to the centroid file to convert camera space coordinates to world space.
