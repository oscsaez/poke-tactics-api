package com.poketactics.clients.test

import io.ktor.http.HttpStatusCode
import io.ktor.http.HttpHeaders
import io.ktor.http.headersOf
import io.ktor.client.HttpClient
import io.ktor.client.plugins.contentnegotiation.ContentNegotiation
import io.ktor.serialization.kotlinx.json.json
import io.ktor.client.engine.mock.*
import kotlinx.serialization.json.Json
import kotlinx.serialization.serializer
import com.poketactics.clients.api.PokeTacticsApiVersion1000CultureneutralPublicKeyTokennullApi as PokeApi
import com.poketactics.clients.PokemonClient
import com.poketactics.clients.models.KeysetPaginationResponse
import com.poketactics.clients.models.PokemonDto
import com.poketactics.clients.models.SpriteDto
import kotlin.test.Test
import kotlin.test.assertEquals
import kotlinx.coroutines.test.runTest

class PokemonClientTest {

    @Test
    fun `getPokemon should return data correctly when API responds 200`() = runTest {
        // Assert
        val expectedPokemonName = "Bulbasaur";

        val expectedResponse = KeysetPaginationResponse(
            items = listOf(
                PokemonDto(
                    name = expectedPokemonName,
                    sprite = SpriteDto()
                )
            )
        );

        val mockEngine = MockEngine { _ ->
            respond(
                content = Json.encodeToString(serializer<KeysetPaginationResponse>(), expectedResponse),
                status = HttpStatusCode.OK,
                headers = headersOf(HttpHeaders.ContentType, "application/json")
            )
        }

        val apiGenerated = PokeApi( 
            httpClientEngine = mockEngine,
            httpClientConfig = { config ->
            config.install(ContentNegotiation) {
                json(Json { 
                    ignoreUnknownKeys = true
                })
            }
        })

        val pokemonClient = PokemonClient(apiGenerated)

        // Act
        val response = pokemonClient.getPokemon(pageSize = 10, lastPokedexOrder = null, lastId = null)

        // Assert
        assertEquals(1, response.items?.size)
        assertEquals(expectedPokemonName, response.items?.first()?.name)
    }
}