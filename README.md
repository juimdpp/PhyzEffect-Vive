# PhyzEffect-Vive

This repository contains the resources, code, and instructions required to set up and execute the PhyzEffect-Vive system, a pipeline for analyzing real-world object interactions and simulating corresponding virtual trajectories.

---

## Steps to Set Up and Use

Follow the steps below to extract and simulate the virtual trajectory from real-world object interactions:

### 1. **Set Up the ZED Camera**
   - Install the ZED Mini or ZED2 camera as required for your setup. Please refer to this [link](https://github.com/stereolabs/zed-sdk/tree/master) for details.
   - Ensure the camera drivers and the ZED SDK are properly installed on your system.
   - Verify the camera functionality using the ZED SDK tools provided by [Stereolabs](https://www.stereolabs.com/).

### 2. **Prepare the Scene**
   - Print at least 3 ArUco markers. In this project, we used the 3 first markers from this [generator](https://chev.me/arucogen/).
   - Place the markers in the scene at distinct positions such that they form a bounding box in 3D space that fully encloses the scene.
   - Ensure the markers are clearly visible and evenly distributed across the scene.

### 3. **Scan the Real World**
   - Use [Scaniverse](https://scaniverse.com/) to create a 3D scan of the scene.
   - Verify that the ArUco markers are clearly visible in the captured scan.

### 4. **Record the Interaction**
   - *(TODO: Add a GUI feature to improve end-user accessibility.)*
   - Use the Unity app provided in the repository:
     1. Set the directory path where the recording will be saved.
     2. Press "Start recording" and perform the object interaction in the scene. Ensure the camera remains stationary.
     3. Stop the recording after completing the interaction.
   - **Note:** Sample recordings used for the final results are included in the repository.

### 5. **Export SVO Video to RGB + Depth PNG Images**
   - Use the provided ZED SDK's SVO export tool to extract RGB and depth images from the video.
   - **Run Command:**

     ```bash
     ./ZED_SVO_Export.exe "path/to/file.svo" "path/to/output/folder"
     ```

   - Converted images for sample recordings are provided in the repository.

### 6. **Analyze Recordings to Extract Bounding Boxes**
   - Use the Python script included in the repository to analyze the recordings:
     1. Install dependencies using `pip install -r requirements.txt`.
     2. Run `3d-annotate.ipynb` and modify the `base_dir` variable for each video.
     3. The script will attempt to detect the ball. For frames where the ball is not detected, manual annotation will be required.
     4. The script generates 3 output files, but only `base_dir_full_annotation.json` is essential.
   - **Note:** Annotated data for sample recordings is provided in the repository.

### 7. **Convert Bounding Boxes to Camera Coordinates**
   - Use the ZED SDK executable to extract centroids of the detected ball:
     - **Run Command:**

       ```bash
       ZED_Object_detection_image_viewer.exe <svo> <inputfile> <outputfile>
       ```

   - This tool processes the bounding box centroids and converts them into camera space coordinates.
   - Instructions for building the executable are available in the [ZED SDK documentation](https://www.stereolabs.com/docs/app-development/cpp/windows). A script to process sample data is also included.

### 8. **Run the Unity Project**
   - Open the Unity project using Unity 2022.3.21f1.
   - Ensure the ZED SDK is installed and properly configured.
   - Load the `MLForSys` scene. Locate the `Optimizer` object, which has three scripts attached:
     - **`SystemManager.cs`**: Manages the overall pipeline, GUI interactions, and data flow.
     - **`RealSystem.cs`**: Processes real-world trajectory data and transforms centroid coordinates from camera space to world space using the ZED Mini camera.
     - **`VirtualSystem.cs`**: Simulates the virtual ball trajectory based on the real-world data.
   - Specify the path to the centroid file to process and simulate the virtual trajectory.
