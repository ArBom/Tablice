# (Pol) Plate Recognition
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
### Detect a plate
![Original photo](https://db3pap004files.storage.live.com/y4mVxxlUvMe_mzDHJA2Q67V5DG1SFVa7tlEnXZjLUKho2lqe6ZgV6A2IoycvfpPdDUO4GBLpmcW2_Apnf8VEsYeq8kLgARRqlzZkKInnY3KAWAh7BITvdG-gBgOaax44T2_GQ_Kb8F-7pEy01TH2Wz6ycLknvRyloWZdj20yTIoXSPeYS2h3kSvFvit7UXScTyI?width=807&height=486&cropmode=none "Original photo")
Original photo

![Photo with blue mask](https://db3pap004files.storage.live.com/y4mWqvNvZNT47uu8IWbtRYuW2Gfpuz_PQUpSXB3huwpQleRspVrjlYh0cil4uFBVYmD_I-yidOsgFne5F7ZN03KvgM_LOJbJZ0IdOR6Vje1L2gc21xNbX3FiHD_FpEHf2Nh5GhhNCP-cH3kAeTiKEd4HG1Q7PittpOhIzDFHqXph0TwcguwpRDP_Cdi7ZxLBpnJ?width=811&height=487&cropmode=none "Photo with blue mask")

![Photo with white mask](https://db3pap004files.storage.live.com/y4mQ8XOoORQ7l9b1FypPIZP2_7rfUdO-A_dMifcImCda8_GtfY2hM-gd2RUzXKVZ6NnwWfJ3Q6iDxzgfKxc03ugVs-lXlG2c-ccwRcETTmpuKLVmh9G7i1T074Zg6iax4bhI3L94yQTEFObrcbzRIMcwJVv8sAc5pznJrDHTEB744fGCJ2A9JHK63yScG8puWkV?width=816&height=486&cropmode=none "Photo with white mask")

![Connected areas](https://db3pap004files.storage.live.com/y4mJpbJUWjUvlRRtvATQQe6xGOWBkiaLTw9aatHvWe70z5R3mJdFmnB35ZTo8DFPhnsiHLh7Lv_HBQFzMW_-gCXJyOt47q18NG3jzJJ2mzutwQk6bsmNk8DnMBA9anBLj6XNN564Q1Yu33heeRIXAhWuLilQryOSgVd-gDyon8rHwdBs9b5BvT1Tklg1PrvPJjY?width=1021&height=610&cropmode=none "Connected areas")

![Seeking corners](https://db3pap004files.storage.live.com/y4m_Sc0r5aOtEV-gsEXTcfweEErdEBdxVgwgcH_Nqpr03Fqb8zKnpltLbdrIN9wagC-ZZVwYk1ekWIWG1zMu_oikR_EGZBqsbM9ujsgGFPsRdIOrCGeBKVF3g7nGnX1bfHMPIkgOVmNrNsBtuwTOID4py0_mIPWLza_5moZkwN9Q2SXjqBhXPLtvPxt_pL70Fab?width=809&height=486&cropmode=none "Seeking corners")
### Abstract a plate
![Plate](https://db5pap001files.storage.live.com/y4msQ2eQw9u1ZrO9XXKmqypzP5-7IqafS6SojmWt6oSAbGxxHKdeNNAhCfB_IJMn5lhXzilkDtHHiqbtc-GNM6F6E4TBV3dglkUc-kQsi0O6y43MOZfHJau1069H6H86oAVEpwcTlxNhVP64L4fGSkje9hWoC2de4YewpQwwDDQU9Ezr7UyM6n_KEx0onVoY6v-?width=515&height=839&cropmode=none "Plate")

### Reading a registration number

Every rectangular area described by right contour is read by SVM algoritm as sign. In the end it is addembled in plate number.

![Read](https://db5pap001files.storage.live.com/y4moQkgyDxLgCKBNJDsxlgLI6UU8TsGiP-J415-ZaRC75SNoB6woKuEE48Cqu7rGFW_3wyQslezgVC998eefDTqFbCV_IvO7kWgnV4bMrfM3WSNAnR79-oqTae-6aeghBMSiGZVtbyeJJCjR_vX9lqD4lRLPKdAGxFjQ-HB7itakA0nF9PN5Jnui0t0X1X1iIFk?width=400&height=477&cropmode=none "Read")

## Status of project
Essentially its done. Single plate is recognited in time less than 1s.

![In_the_end](https://db5pap001files.storage.live.com/y4mLnzOiSU_EBv3kJCKY_MpoM5L1eXmyu787UHy_viq3yQS6M2Qhd2_zOaDTohMpHcRuzxXFOPMprS2wEVeygkC_SgBSCy6NFoRaAxtBtQ1IB2v1p3cgl5CHzCWI4b7tFac-THUf6Ff2NPkqSewk029c_lTgYV4ZzCEqYH5ZciV25Ma3KiWOGyGe22de1ktD-Ug?width=508&height=161&cropmode=none "In the end")


#### At a pinch To-Do:
* optimalization
* find and repair bugs
