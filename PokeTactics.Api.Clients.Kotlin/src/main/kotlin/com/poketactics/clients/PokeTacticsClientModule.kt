import com.poketactics.clients.api.PokeTacticsApiVersion1000CultureneutralPublicKeyTokennullApi as PokeTacticsClient
import dagger.Module
import dagger.Provides
import dagger.hilt.InstallIn
import dagger.hilt.components.SingletonComponent
import javax.inject.Singleton

@Module
@InstallIn(SingletonComponent::class)
object PokeTacticsClientModule {

    @Provides
    @Singleton
    fun providesPokeTacticsApi(): PokeTacticsClient {
        return PokeTacticsClient();
    }

    @Provides
    fun providePokemonClient(client: PokeTacticsClient): IPokemonClient {
        return PokemonClient(client);
    }
}