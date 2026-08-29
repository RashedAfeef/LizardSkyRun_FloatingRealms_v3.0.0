# دليل البناء والتصدير

## إعداد المشروع

من قائمة Unity اختر `Lizard Sky Run > Apply Recommended Settings`. يضبط ذلك اسم المنتج، الاتجاه الأفقي، معدل الإطارات، معرّفات الحزمة الافتراضية، والمشهد الموجود في Build Settings.

## Android

1. ثبّت Android Build Support من Unity Hub، بما في ذلك SDK وNDK وOpenJDK.
2. افتح `File > Build Profiles` واختر Android ثم `Switch Platform`.
3. غيّر `Company Name` و`Package Name` إلى القيم الخاصة بك.
4. للنشر في Google Play اختر AAB وأنشئ Keystore خاصاً بك ثم Build.

## iOS

1. ثبّت iOS Build Support.
2. اختر iOS من Build Profiles ثم Build.
3. افتح مشروع Xcode الناتج على جهاز macOS، وحدد فريق Apple والتوقيع ثم ابنِ التطبيق.

## Web

اختر Web من Build Profiles، ثم Build. الواجهة تدعم الفأرة والسحب، لكن يُنصح باختبار الأداء على الهواتف المستهدفة.

## ضبط الصعوبة

افتح `RunnerConfig.cs` وعدّل القيم الافتراضية، أو أنشئ أصل إعدادات من `Create > Lizard Sky Run > Runner Config` ثم اربطه بحقل Config في كائن `GameBootstrap`.

أهم القيم: سرعة البداية والحد الأعلى والتسارع، تباعد المسارات، طول مقطع الطريق، عدد المقاطع، ارتفاع القفزة ومدة الانزلاق، مدة ونطاق المغناطيس، مدة Pulse Board و2X Boost، تدرج المضاعف، واحتمال ظهور القدرات.

## أداء الشخصية

الشخصية النشطة هي ملف Meshy FBX عالي التفاصيل مع خامة أصلية كبيرة، ولذلك تحتاج إلى اختبار فعلي على الجهاز المستهدف. ضغط الاستيراد يقلل حجم الذاكرة والتنزيل لكنه لا يخفض عدد مثلثات الشبكة.

للإصدار النهائي على هاتف ضعيف:

1. اختبر النسخة الحالية على الجهاز المستهدف باستخدام Unity Profiler.
2. أنشئ نسخة Remesh بين 15,000 و20,000 مثلث مع الحفاظ على Humanoid Rig وأسماء الحركات.
3. استبدل `JordanianHero_Meshy_Animated_Full.fbx` و`JordanianHero_BaseColor.png` مع الحفاظ على الاسمين والمسارين.
4. اضغط `Lizard Sky Run > Validate Project` وتحقق من المقاطع والـAvatar والخامة.

اختبر على جهاز Android فعلي قبل رفع جودة الظلال أو إضافة Post Processing.
