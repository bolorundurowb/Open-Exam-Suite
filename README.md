# Open Exam Suite
[![Build status](https://ci.appveyor.com/api/projects/status/ll8d5i7l6f2p9siw?svg=true)](https://ci.appveyor.com/project/BolorunduroWinnerTimothy/open-exam-suite)   [![Download Open Exam Suite](https://img.shields.io/sourceforge/dm/open-exam-suite.svg)](https://sourceforge.net/projects/open-exam-suite/files/latest/download) [![NETFramework](https://img.shields.io/badge/.net-4.0-0066b6.svg)](https://www.microsoft.com/en-us/download/details.aspx?id=17851) [![License](https://img.shields.io/badge/license-GPLv3-orange.svg)]()

Please join the discussion about the future of this project [here](https://github.com/bolorundurowb/Open-Exam-Suite/issues/85)

__The former aim to convert existing `VCE` exams to the `OEF` format has been dropped. If you want any feature in the next release, create an [issue](https://github.com/bolorundurowb/Open-Exam-Suite/issues) here on Github or hit me up on twitter [@Mr_Bolorunduro](https://twitter.com/Mr_Bolorunduro).__ **If this project has been of benefit to you, feel free to give it a star so others can find it too.**

This project is an answer to a call to develop an open source alternative to Avanset's Visual CertExam Suite. This project has an exam designer that creates an *oef* (Open Exam Format) file and an exam simulator that can read and simulate the exam. It aims to be the open source solace for those wanting to take computer based simulated examinations.
Answering the call to design an open source alternative to Avanset's Visual CertExam Suite birthed version one of open exam suite and as I was still learning and getting a hang of the language, the code written is less than ideal which gave birth to version two of the software which based on the new knowledge was better written and had new features. The code resides [here](https://github.com/bolorundurowb/Open-Exam-Suite/tree/version2).

[NB: the version 1 and 2 *oef* files are basically just zip files that contain an XML file and images. The XML contains the exam details and structure while the pictures are question images]

***Please, I am working on a site where exams that have been compiles using this tool can be shared with the community. Until I'm done, please send them to ogatimo@gmail.com and I'll post them on the product documentation/help page.*** *[http://bolorundurowb.github.io/Open-Exam-Suite](http://bolorundurowb.github.io/Open-Exam-Suite)*


## Changelog

### Version 3.1 
Version 3.1 keeps backward compatibility with v3.0 exams (obviously), but aims to run better and faster. The upgrades affect every part of the app from the shared library to the exam converter, the creator and the simulator.

#### General improvements
1. Support for questions with multiple answers has been added.

#### Creator Section
1. Support for exporting exams as `JSON` has be added.
2. Support for exporting exams as `XML` has be added.


### Version 3 added features
Version three does not have backward support for exams created with the versions 1 and 2 creator apps. But I have written and added a converter that can easily upgrade the old exam files to the newer form that is readable by the version 3 simulator.
#### General improvements
1. Coded using every OOP principle I know.
2. Coded using TDD.
3. Exam file (*oef*) type changed from *zip* based to a serializable binary.

#### Creator Section
1. UI overhaul to make it even more intuitive and better.
2. Faster response
3. Support for Undo and Redo.
4. Support for Copying, Cutting and Pasting.
5. Support documentation in the help section.

#### Simulator Section
1. Full rewrite for stability sake.


### Version 2 
#### Creator Section
1. Supports adding of explanations to questions.
2. Written utilizing more OOP principles than version 1.0
3. Less buggy and faster.
4. New, more stable and intuitive UI.

#### Simulator Section
1. Support for checking correct answer while taking exams.
2. Support for viewing an answers explanation.
3. Support for printing results.

I have gained more programming knowledge and experience and intend to use that in version three of this project. Version three would be faster and have more features than version two. It aims to be a full rewrite.


### Version 1 features
#### Creator Section
1. Supports grouping questions into sections.
2. Supports Images in questions
3. Supports an unlimited number of options per question.
4. Supports importing an existing exam file.
5. Supports editing an exam file.
6. Supports time limits in exams.

#### Simulator Section
1. Supports taking exams as designed.
2. Supports selecting certain sections to take.
3. Supports just selecting the number of questions to be taken.
4. Supports changing of time limits.


## Download
### SourceForge
You can get the complete suite in an installer from [here](https://sourceforge.net/projects/open-exam-suite).
