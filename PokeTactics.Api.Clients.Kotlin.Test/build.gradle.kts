plugins {
    kotlin("jvm")
    kotlin("plugin.serialization")
    id("org.jetbrains.kotlinx.kover")
}

repositories {
    mavenCentral()
    google()
}

dependencies {
    val ktorVersion = "2.3.7"
    val coroutinesVersion = "1.7.3"

    implementation(project(":PokeTactics.Api.Clients.Kotlin"))

    implementation("io.ktor:ktor-client-core:$ktorVersion")
    implementation("io.ktor:ktor-serialization-kotlinx-json:$ktorVersion")
    implementation("io.ktor:ktor-client-content-negotiation:$ktorVersion")

    testImplementation("org.jetbrains.kotlin:kotlin-test")
    testImplementation("io.ktor:ktor-client-mock:$ktorVersion")
    testImplementation("org.jetbrains.kotlinx:kotlinx-coroutines-test:$coroutinesVersion")

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