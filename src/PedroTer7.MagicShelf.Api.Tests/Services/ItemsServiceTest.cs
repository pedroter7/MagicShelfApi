using AutoMapper;
using Bogus;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PedroTer7.MagicShelf.Api.Data.Config.Mappings;
using PedroTer7.MagicShelf.Api.Data.Dtos;
using PedroTer7.MagicShelf.Api.Data.Exceptions;
using PedroTer7.MagicShelf.Api.Data.Repositories.Interfaces;
using PedroTer7.MagicShelf.Api.Service.Config.Mappings;
using PedroTer7.MagicShelf.Api.Service.Config.Options;
using PedroTer7.MagicShelf.Api.Service.Dtos;
using PedroTer7.MagicShelf.Api.Service.Exceptions;
using PedroTer7.MagicShelf.Api.Service.Services.Implementations;
using PedroTer7.MagicShelf.Api.Tests.Util;

namespace PedroTer7.MagicShelf.Api.Tests.Services
{
    public class ItemsServiceTest
    {
        private readonly Faker _faker;
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;
        private readonly IOptions<ShelfOptions> _shelfIOptions;

        private ItemService CreateServiceInstance() => new(_itemRepository, _mapper, _shelfIOptions);

        public ItemsServiceTest()
        {
            _faker = new Faker();
            _itemRepository = Substitute.For<IItemRepository>();
            _mapper = CreateAutoMapperInstance();
            _shelfIOptions = Substitute.For<IOptions<ShelfOptions>>();
        }

        private static IMapper CreateAutoMapperInstance()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new DataToServiceMappingProfile());
                mc.AddProfile(new ServiceToDataMappingProfile());
            });
            return mappingConfig.CreateMapper();
        }

        private void ConfigureShelfOptions(int shelfSize)
        {
            _shelfIOptions.Value.Returns(new ShelfOptions
            {
                ShelfSize = shelfSize
            });
        }

        private ItemOutDto GenerateRandomItemOutDto() => new()
        {
            DateStored = _faker.Date.Past(),
            Description = _faker.Lorem.Text(),
            Id = _faker.Random.Long(1),
            Name = _faker.Lorem.Word(),
            NumberCommentsAbout = _faker.Random.Int(0)
        };

        private IList<ItemOutDto> GenerateRandomItemOutDtos() 
            => _faker.Make(_faker.Random.Int(1, 500), GenerateRandomItemOutDto);
        private IList<ItemOutDto> GenerateRandomItemDtosWithKeywordInSomeItemsNames(string keyword, out int nWithKeyword)
        {
            var items = GenerateRandomItemOutDtos();
            nWithKeyword = CollectionUtils
                .ApplyModificationToSomeItemsInCollection(items, i => i.Name = $"{i.Name} {keyword}");
            return items;
        }

        private IList<ItemOutDto> GenerateRandomItemDtosWithKeywordInSomeItemsDescriptions(string keyword, out int nWithKeyword)
        {
            var items = GenerateRandomItemOutDtos();
            nWithKeyword = CollectionUtils
                .ApplyModificationToSomeItemsInCollection(items, i => i.Description = $"{i.Description} {keyword}");
            return items;
        }

        private ItemToInsertDto GenerateRandomItemToInsertDbo() => new()
        {
            Content = _faker.Lorem.Text(),
            Description = _faker.Lorem.Text(),
            Name = _faker.Lorem.Sentence()
        };

        private CommentOutDto GenerateRandomCommentOutDto() => new()
        {
            Author = _faker.Person.FullName,
            Id = _faker.Random.Long(1),
            Text = _faker.Lorem.Text(),
        };

        private IList<CommentOutDto> GenerateRandomCommentOutDtos()
            => _faker.Make(_faker.Random.Int(1, 300), GenerateRandomCommentOutDto);

        private ItemCommentToInsertDto GenerateItemCommentToInsertDto() => new()
        {
            Author = _faker.Person.FullName,
            Text = _faker.Lorem.Text()
        };

        [Fact(DisplayName = "There is room for new items")]
        [Trait("unit", nameof(ItemService.ThereIsRoomForNewItems))]
        public async Task Test_ThereIsRoomForNewItems_WhenThereIsRoom()
        {
            // Arrange
            var shelfSize = _faker.Random.Int(1);
            _itemRepository.CountStoredItems().Returns(_faker.Random.Int(0, shelfSize - 1));
            ConfigureShelfOptions(shelfSize);
            var service = CreateServiceInstance();

            // Act
            var result = await service.ThereIsRoomForNewItems();

            // Assert
            Assert.True(result);
            await _itemRepository.Received(1).CountStoredItems();
        }

        [Fact(DisplayName = "There is no room for new items")]
        [Trait("unit", nameof(ItemService.ThereIsRoomForNewItems))]
        public async Task Test_ThereIsRoomForNewItems_WhenThereIsNoRoom()
        {
            // Arrange
            var shelfSize = _faker.Random.Int(1);
            _itemRepository.CountStoredItems().Returns(shelfSize);
            ConfigureShelfOptions(shelfSize);
            var service = CreateServiceInstance();

            // Act
            var result = await service.ThereIsRoomForNewItems();

            // Assert
            Assert.False(result);
            await _itemRepository.Received(1).CountStoredItems();
        }

        [Fact(DisplayName = "When there is no items on the shelf must throw proper exception")]
        [Trait("unit", nameof(ItemService.ListAllItems))]
        public async Task Test_ListAllItems_ThereAreNoItems_ShouldThrow()
        {
            // Arrange
            _itemRepository.GetAll().ThrowsAsync(new EntityNotFoundException("Item", "all"));
            var service = CreateServiceInstance();

            // Act
            // Assert
            await Assert.ThrowsAsync<EmptyShelfException>(() => service.ListAllItems());
            await _itemRepository.Received(1).GetAll();
        }

        [Fact(DisplayName = "List all items when there are items")]
        [Trait("unit", nameof(ItemService.ListAllItems))]
        public async Task Test_ListAllItems_TherAreItems()
        {
            // Arrange
            var itemsList = GenerateRandomItemOutDtos();
            _itemRepository.GetAll().Returns(itemsList);
            var service = CreateServiceInstance();

            // Act
            var items = await service.ListAllItems();

            // Assert
            Assert.Equal(itemsList.Count, items.Count());
            await _itemRepository.Received(1).GetAll();
        }

        [Fact(DisplayName = "List items that contain keyword in their names")]
        [Trait("unit", nameof(ItemService.ListItemsThatContainKeyword))]
        public async Task Test_ListItemsThatContainKeyword_ThereAreItemsWithKeywordInTheirNames()
        {
            // Arrange
            var keyword = _faker.Lorem.Sentence(10);
            var itemsList = GenerateRandomItemDtosWithKeywordInSomeItemsNames(keyword, out var nWIthKeyword);
            _itemRepository.GetAll().Returns(itemsList);
            var service = CreateServiceInstance();

            // Act
            var itemsReturned = await service.ListItemsThatContainKeyword(keyword);

            // Assert
            Assert.Equal(nWIthKeyword, itemsReturned.Count());
            await _itemRepository.Received(1).GetAll();
        }

        [Fact(DisplayName = "List items that contain keyword in their descriptions")]
        [Trait("unit", nameof(ItemService.ListItemsThatContainKeyword))]
        public async Task Test_ListItemsThatContainKeyword_ThereAreItemsWithKeywordInTheirDescription()
        {
            // Arrange
            var keyword = _faker.Lorem.Sentence(10);
            var itemsList = GenerateRandomItemDtosWithKeywordInSomeItemsDescriptions(keyword, out var nWIthKeyword);
            _itemRepository.GetAll().Returns(itemsList);
            var service = CreateServiceInstance();

            // Act
            var itemsReturned = await service.ListItemsThatContainKeyword(keyword);

            // Assert
            Assert.Equal(nWIthKeyword, itemsReturned.Count());
            await _itemRepository.Received(1).GetAll();
        }

        [Fact(DisplayName = "List items that contain keyword when there is no item with keyword should throw proper exception")]
        [Trait("unit", nameof(ItemService.ListItemsThatContainKeyword))]
        public async Task Test_ListItemsThatContainKeyword_ThereAreNoItemsWithKeyword_ShouldThrow()
        {
            // Arrange
            var keyword = _faker.Lorem.Text();
            var itemsList = GenerateRandomItemOutDtos();
            _itemRepository.GetAll().Returns(itemsList);
            var service = CreateServiceInstance();

            // Act
            // Assert
            await Assert.ThrowsAsync<NoItemOnShelfSatisfiesConditionException>(
                () => service.ListItemsThatContainKeyword(keyword));
            await _itemRepository.Received(1).GetAll();
        }

        [Fact(DisplayName = "List items that contain keyword when there are no itens should throw proper exception")]
        [Trait("unit", nameof(ItemService.ListItemsThatContainKeyword))]
        public async Task Test_ListItemsThatContainKeyword_ThereAreNoItems_ShouldThrow()
        {
            // Arrange
            var keyword = _faker.Lorem.Text();
            _itemRepository.GetAll().ThrowsAsync(new EntityNotFoundException("Item", "all"));
            var service = CreateServiceInstance();

            // Act
            // Assert
            await Assert.ThrowsAsync<EmptyShelfException>(
                () => service.ListItemsThatContainKeyword(keyword));
            await _itemRepository.Received(1).GetAll();
        }

        [Fact(DisplayName = "Pop an item that is on the shelf")]
        [Trait("unit", nameof(ItemService.PopItem))]
        public async Task Test_PopItem_ItemOnShelf()
        {
            // Arrange
            var itemDto = GenerateRandomItemOutDto();
            var itemId = itemDto.Id;
            var itemContent = _faker.Lorem.Text();
            _itemRepository.GetItem(itemId).Returns(itemDto);
            _itemRepository.GetItemContent(itemId).Returns(itemContent);
            var service = CreateServiceInstance();

            // Act
            var result = await service.PopItem(itemId);

            // Assert
            Assert.Equal(itemId, result.Id);
            Assert.Equal(itemContent, result.Content);
            await _itemRepository.Received(1).GetItem(itemId);
            await _itemRepository.Received(1).GetItemContent(itemId);
            await _itemRepository.DidNotReceive().GetItemContent(Arg.Is<long>(x => x != itemId));
            await _itemRepository.DidNotReceive().GetItem(Arg.Is<long>(x => x != itemId));
            await _itemRepository.Received(1).RemoveItem(itemId);
            await _itemRepository.DidNotReceive().RemoveItem(Arg.Is<long>(x => x != itemId));
        }

        [Fact(DisplayName = "Pop an item that is not on the shelf should throw proper exception")]
        [Trait("unit", nameof(ItemService.PopItem))]
        public async Task Test_PopItem_ItemNotOnShelf_ShouldThrow()
        {
            // Arrange
            var itemId = _faker.Random.Long(1);
            _itemRepository.GetItem(itemId).ThrowsAsync(new EntityNotFoundException("Item", $"Id={itemId}"));
            var service = CreateServiceInstance();

            // Act
            // Assert
            await Assert
                .ThrowsAsync<NoItemOnShelfSatisfiesConditionException>(() => service.PopItem(itemId));
            await _itemRepository.Received(1).GetItem(itemId);
            await _itemRepository.DidNotReceiveWithAnyArgs().GetItemContent(default);
            await _itemRepository.DidNotReceive().GetItem(Arg.Is<long>(x => x != itemId));
            await _itemRepository.DidNotReceiveWithAnyArgs().RemoveItem(default);
        }

        [Fact(DisplayName = "Insert item correclty")]
        [Trait("unit", nameof(ItemService.InsertItem))]
        public async Task Test_InserItem_ShouldWork()
        {
            // Arrange
            var itemToInsert = GenerateRandomItemToInsertDbo();
            var shelfSize = _faker.Random.Int(1);
            var idToBeGenerated = _faker.Random.Long(1);
            _itemRepository.CountStoredItems().Returns(shelfSize - 1);
            _itemRepository.StoreItem(Arg.Is<ItemInDto>(x => x.Content.Equals(itemToInsert.Content)))
                .Returns(idToBeGenerated);
            ConfigureShelfOptions(shelfSize);
            var service = CreateServiceInstance();

            // Act
            var result = await service.InsertItem(itemToInsert);

            // Assert
            Assert.Equal(idToBeGenerated, result);
            await _itemRepository.Received(1).CountStoredItems();
        }

        [Fact(DisplayName = "Inserting an item when shelf is full should throw proper exception")]
        [Trait("unit", nameof(ItemService.InsertItem))]
        public async Task Test_InserItem_ShelfIsFull_ShouldThrow()
        {
            // Arrange
            var itemToInsert = GenerateRandomItemToInsertDbo();
            var shelfSize = _faker.Random.Int(1);
            var idToBeGenerated = _faker.Random.Long(1);
            _itemRepository.CountStoredItems().Returns(shelfSize);
            ConfigureShelfOptions(shelfSize);
            var service = CreateServiceInstance();

            // Act
            // Assert
            await Assert.ThrowsAsync<ShelfFullException>(() => service.InsertItem(itemToInsert));
            await _itemRepository.Received(1).CountStoredItems();
            await _itemRepository.DidNotReceive().StoreItem(Arg.Any<ItemInDto>());
        }

        [Fact(DisplayName = "Get comments for item when item has comments")]
        [Trait("unit", nameof(ItemService.GetCommentsForItem))]
        public async Task Test_GetCommentsForItem_ItemHasComments()
        {
            // Arrange
            var itemOutDto = GenerateRandomItemOutDto();
            var itemId = itemOutDto.Id;
            _itemRepository.GetItem(itemId).Returns(itemOutDto);
            var commentOutDtos = GenerateRandomCommentOutDtos();
            _itemRepository.GetCommentsForItem(itemId).Returns(commentOutDtos);
            var service = CreateServiceInstance();

            // Act
            var result = await service.GetCommentsForItem(itemId);

            // Assert
            Assert.Equal(commentOutDtos.Count, result.Count());
            await _itemRepository.Received(1).GetItem(itemId);
            await _itemRepository.DidNotReceive().GetItem(Arg.Is<long>(x => x != itemId));
            await _itemRepository.Received(1).GetCommentsForItem(itemId);
            await _itemRepository.DidNotReceive().GetCommentsForItem(Arg.Is<long>(x => x != itemId));
        }

        [Fact(DisplayName = "Get comments for item when item does not exist should throw proper exception")]
        [Trait("unit", nameof(ItemService.GetCommentsForItem))]
        public async Task Test_GetCommentsForItem_ItemDoesNotExist_ShouldThrow()
        {
            // Arrange
            var itemId = _faker.Random.Long(1);
            _itemRepository.GetItem(Arg.Any<long>())
                .ThrowsAsync(new EntityNotFoundException("Item", "all"));

            var service = CreateServiceInstance();

            // Act
            // Assert
            await Assert.ThrowsAsync<NoItemOnShelfSatisfiesConditionException>(
                () => service.GetCommentsForItem(itemId));
            await _itemRepository.Received(1).GetItem(itemId);
            await _itemRepository.DidNotReceive().GetItem(Arg.Is<long>(x => x != itemId));
            await _itemRepository.DidNotReceive().GetCommentsForItem(Arg.Any<long>());
        }

        [Fact(DisplayName = "Get comments for item when there are no comments should throw proper exception")]
        [Trait("unit", nameof(ItemService.GetCommentsForItem))]
        public async Task Test_GetCommentsForItem_ItemDoesNotHaveComments_ShouldThrow()
        {
            // Arrange
            var itemOutDto = GenerateRandomItemOutDto();
            var itemId = itemOutDto.Id;
            _itemRepository.GetItem(itemId).Returns(itemOutDto);
            _itemRepository.GetCommentsForItem(Arg.Any<long>())
                .ThrowsAsync(new EntityNotFoundException("Item", "all"));
            var service = CreateServiceInstance();

            // Act
            // Assert
            await Assert.ThrowsAsync<ItemHasNoCommentsException>(
                () => service.GetCommentsForItem(itemId));
            await _itemRepository.Received(1).GetItem(itemId);
            await _itemRepository.DidNotReceive().GetItem(Arg.Is<long>(x => x != itemId));
            await _itemRepository.Received(1).GetCommentsForItem(itemId);
            await _itemRepository.DidNotReceive().GetCommentsForItem(Arg.Is<long>(x => x != itemId));
        }

        [Fact(DisplayName = "Insert comment for item when item exists")]
        [Trait("unit", nameof(ItemService.InsertCommentForItem))]
        public async Task Test_InsertCommentForItem_ItemExists_ShouldWork()
        {
            // Arrange
            var itemOutDto = GenerateRandomItemOutDto();
            var itemId = _faker.Random.Long(1);
            var commentToInsert = GenerateItemCommentToInsertDto();
            var idToGenerate = _faker.Random.Long(1);
            _itemRepository.AddCommentToItem(itemId, 
                Arg.Is<CommentInDto>(dto => dto.Text.Equals(commentToInsert.Text)))
                .Returns(idToGenerate);
            var service = CreateServiceInstance();

            // Act
            var result = await service.InsertCommentForItem(itemId, commentToInsert);

            // Assert
            Assert.Equal(idToGenerate, result);
        }

        [Fact(DisplayName = "Insert comment for item when item does not exist should throw proper exception")]
        [Trait("unit", nameof(ItemService.InsertCommentForItem))]
        public async Task Test_InsertCommentForItem_ItemDoesNotExist_ShouldThrow()
        {
            // Arrange
            var itemId = _faker.Random.Long(1);
            _itemRepository.AddCommentToItem(Arg.Any<long>(), Arg.Any<CommentInDto>())
                .ThrowsAsync(new EntityNotFoundException("Item", "all"));

            var commentToInsert = GenerateItemCommentToInsertDto();
            var service = CreateServiceInstance();

            // Act
            // Assert
            await Assert.ThrowsAsync<NoItemOnShelfSatisfiesConditionException>(
                () => service.InsertCommentForItem(itemId, commentToInsert));
        }
    }
}
