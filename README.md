# (🇵🇱) Plate Recognition
Console App created in MS Visual Studio 2017 enables one to read registracion number from graphic file.
## Table of contents
* [Technologies](#Technologies)
* [Algorithm](#Alorithm)
* [Status of project](#Status_of_project)

## Technologies
Project is created with:
* C# (.Net Framework 4.7.2)
* EmguCV (ver. 4.4.0.4061)
```
PM> Install-Package Emgu.CV -Version 4.4.0.4061
```

## Algorithm
### 1. Detect a plate
![Original photo](https://github.com/user-attachments/assets/43b3d7bc-ff0f-4812-8e05-b3830f0aa0a9)

Original photo with plate

#### 1.1
![Photo with blue mask](https://github.com/user-attachments/assets/809f1abe-9640-4df6-aa59-f4a107a99c66)

A blue mask is applied to the original image...

#### 1.2
![Photo with white mask](https://github.com/user-attachments/assets/52029116-4b52-4494-8a9a-206a6ea1753a)

...and a white one.

#### 1.3
![Connected areas](https://github.com/user-attachments/assets/b5cfd2d0-ec20-40fe-aa1d-d5c2c321c233)

Both images (1.1 and 1.2) are merged into one with possible areas of white part of plate.

#### 1.4
![Seeking corners](https://github.com/user-attachments/assets/e14f00fc-0b6f-44ad-b1c4-43fa73fc24cc)

The program searches for vertices of biggest area (of image 1.3)

### 2. Abstract a plate
![Plate](https://github.com/user-attachments/assets/4d4fe0a8-b469-4a16-87e3-1f2101badb53)

Program dissects the white part of image from image 1.3 with use the corners found at image 1.4.
Then it reshape plate to the best possible to read data.
In the end of this step it mark the contours of every single sign.

### 3. Reading a registration number

![Read](https://github.com/user-attachments/assets/b44f1c1d-8e55-4f87-9254-833c49bb3a27)

Every rectangular area described by contour (in the end of 2.) is read by SVM algorithm assign. In the end they are assembled together into plate number.

## Status of project
Essentially its done. Single plate is recognited in time less than 1s.

![show it!](https://github.com/user-attachments/assets/36489a61-9f2f-42e3-87a4-17cda7cd4aab)
