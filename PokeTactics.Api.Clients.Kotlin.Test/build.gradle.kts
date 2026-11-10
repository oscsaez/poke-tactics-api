plugins {
    alias(libs.plugins.kotlin.jvm)
    alias(libs.plugins.kotlin.serialization)
    alias(libs.plugins.kover)
}

repositories {
    mavenCentral()
    google()
}

dependencies {
    implementation(project(":PokeTactics.Api.Clients.Kotlin"))

    implementation(libs.ktor.client.core)
    implementation(libs.ktor.serialization.kotlinx.json)
    implementation(libs.ktor.client.content.negotiation)

    testImplementation(libs.kotlin.test)
    testImplementation(libs.ktor.client.mock)
    testImplementation(libs.coroutines.test)
    testImplementation(libs.junit.jupiter)
    testRuntimeOnly(libs.junit.platform.launcher)

    kover(project(":PokeTactics.Api.Clients.Kotlin"))
}

tasks.test {
    useJUnitPlatform()
    finalizedBy("koverXmlReport")
}

koverReport {
    filters {
        excludes {
            packages("com.poketactics.clients.api", "com.poketactics.clients.models")
        }
    }
}