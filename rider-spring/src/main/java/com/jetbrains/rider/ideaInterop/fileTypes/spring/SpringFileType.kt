package com.jetbrains.rider.ideaInterop.fileTypes.spring

import com.intellij.openapi.fileTypes.LanguageFileType

object SpringFileType : LanguageFileType(SpringLanguage) {
    override fun getName() = "Spring"
    override fun getDefaultExtension() = "pas"
    override fun getDescription() = "Spring file"
    override fun getIcon() = null
}
